// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpSigning
{
    public abstract class SigningKey
    {
        public SigningKey(Jwk jwk)
        {
            if (jwk == null) throw new ArgumentNullException("jwk");
            Jwk = jwk;
        }

        public Jwk Jwk { get; protected set; }
        public abstract Signature ToSignature();
    }

    public class SymmetricKey : SigningKey
    {
        public SymmetricKey(Jwk jwk) : base(jwk)
        {
            Read();
        }

        public byte[] KeyBytes { get; private set; }

        void Read()
        {
            if (String.IsNullOrWhiteSpace(Jwk.k))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.Symmetric.KeyProperty);
            }

            if (!HttpSigningConstants.Jwk.Symmetric.Algorithms.Contains(Jwk.alg))
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.Jwk.AlgorithmProperty);
            }

            KeyBytes = Base64Url.Decode(Jwk.k);
        }

        public override Signature ToSignature()
        {
            switch(Jwk.alg)
            {
                case "HS256": return new HS256Signature(KeyBytes);
                case "HS384": return new HS384Signature(KeyBytes);
                case "HS512": return new HS512Signature(KeyBytes);
            }

            throw new InvalidOperationException("Invalid algorithm");
        }
    }

    public class RSASigningKey : SigningKey
    {
        public RSASigningKey(Jwk jwk) : base(jwk)
        {
            Read();
        }

        public byte[] ModulusBytes { get; private set; }
        public byte[] ExponentBytes { get; private set; }

        void Read()
        {
            if (String.IsNullOrWhiteSpace(Jwk.n))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.RSA.ModulusProperty);
            }
            if (String.IsNullOrWhiteSpace(Jwk.e))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.RSA.ExponentProperty);
            }

            if (!HttpSigningConstants.Jwk.RSA.Algorithms.Contains(Jwk.alg))
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.Jwk.AlgorithmProperty);
            }

            ModulusBytes = Base64Url.Decode(Jwk.n);
            ExponentBytes = Base64Url.Decode(Jwk.e);
        }

        public override Signature ToSignature()
        {
            switch (Jwk.alg)
            {
                case "RS256": return new RS256Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
                case "RS384": return new RS384Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
                case "RS512": return new RS512Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
            }

            throw new InvalidOperationException("Invalid algorithm");
        }
    }
}
