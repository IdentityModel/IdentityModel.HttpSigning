// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel.HttpSigning.Logging;
using IdentityModel.Jwt;
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
        public SigningKey(JsonWebKey jwk)
        {
            if (jwk == null) throw new ArgumentNullException("jwk");

            Jwk = jwk;
        }

        public JsonWebKey Jwk { get; protected set; }
        public abstract Signature ToSignature();
    }

    public class SymmetricKey : SigningKey
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public SymmetricKey(JsonWebKey jwk) : base(jwk)
        {
            Read();
        }

        public byte[] KeyBytes { get; private set; }

        void Read()
        {
            if (String.IsNullOrWhiteSpace(Jwk.K))
            {
                Logger.Error("Missing " + HttpSigningConstants.Jwk.Symmetric.KeyProperty);
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.Symmetric.KeyProperty);
            }

            if (!HttpSigningConstants.Jwk.Symmetric.Algorithms.Contains(Jwk.Alg))
            {
                Logger.Error("Invalid " + HttpSigningConstants.Jwk.AlgorithmProperty);
                throw new ArgumentException("Invalid " + HttpSigningConstants.Jwk.AlgorithmProperty);
            }

            KeyBytes = Base64Url.Decode(Jwk.K);
        }

        public override Signature ToSignature()
        {
            switch(Jwk.Alg)
            {
                case "HS256": return new HS256Signature(KeyBytes);
                case "HS384": return new HS384Signature(KeyBytes);
                case "HS512": return new HS512Signature(KeyBytes);
            }

            Logger.Error("Invalid algorithm: " + Jwk.Alg);
            throw new InvalidOperationException("Invalid algorithm");
        }
    }

    public class RSAPublicKey : SigningKey
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public RSAPublicKey(JsonWebKey jwk) : base(jwk)
        {
            Read();
        }

        public byte[] ModulusBytes { get; private set; }
        public byte[] ExponentBytes { get; private set; }

        void Read()
        {
            if (String.IsNullOrWhiteSpace(Jwk.N))
            {
                Logger.Error("Missing " + HttpSigningConstants.Jwk.RSA.ModulusProperty);
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.RSA.ModulusProperty);
            }
            if (String.IsNullOrWhiteSpace(Jwk.E))
            {
                Logger.Error("Missing " + HttpSigningConstants.Jwk.RSA.ExponentProperty);
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.RSA.ExponentProperty);
            }

            if (!HttpSigningConstants.Jwk.RSA.Algorithms.Contains(Jwk.Alg))
            {
                Logger.Error("Invalid " + HttpSigningConstants.Jwk.AlgorithmProperty);
                throw new ArgumentException("Invalid " + HttpSigningConstants.Jwk.AlgorithmProperty);
            }

            ModulusBytes = Base64Url.Decode(Jwk.N);
            ExponentBytes = Base64Url.Decode(Jwk.E);
        }

        public override Signature ToSignature()
        {
            switch (Jwk.Alg)
            {
                case "RS256": return new RS256Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
                case "RS384": return new RS384Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
                case "RS512": return new RS512Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
            }

            Logger.Error("Invalid algorithm: " + Jwk.Alg);
            throw new InvalidOperationException("Invalid algorithm");
        }
    }
}
