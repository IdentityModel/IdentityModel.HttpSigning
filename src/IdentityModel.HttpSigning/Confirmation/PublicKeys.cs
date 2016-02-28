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
    public abstract class PublicKey
    {
        public PublicKey(Jwk jwk)
        {
            if (jwk == null) throw new ArgumentNullException("jwk");
            Jwk = jwk;
        }

        public Jwk Jwk { get; protected set; }
        public abstract Signature ToSignature();
    }

    public class HSPublicKey : PublicKey
    {
        public HSPublicKey(Jwk jwk) : base(jwk)
        {
            Read(Jwk.Contents);
        }

        public string KeyString { get; private set; }
        public byte[] KeyBytes { get; private set; }

        void Read(IDictionary<string, object> jwk)
        {
            if (!jwk.ContainsKey(HttpSigningConstants.HS.KeyProperty))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.HS.KeyProperty);
            }

            KeyString = jwk[HttpSigningConstants.HS.KeyProperty] as string;

            if (KeyString == null)
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.HS.KeyProperty);
            }

            KeyBytes = Base64Url.Decode(KeyString);
        }

        public override Signature ToSignature()
        {
            switch(Jwk.Algorithm)
            {
                case "HS256": return new HS256Signature(KeyBytes);
                case "HS384": return new HS384Signature(KeyBytes);
                case "HS512": return new HS512Signature(KeyBytes);
            }

            throw new InvalidOperationException("Invalid algorithm");
        }
    }

    public class RSPublicKey : PublicKey
    {
        public RSPublicKey(Jwk jwk) : base(jwk)
        {
            Read(jwk.Contents);
        }

        public string ModulusString { get; private set; }
        public byte[] ModulusBytes { get; private set; }
        public string ExponentString { get; private set; }
        public byte[] ExponentBytes { get; private set; }

        void Read(IDictionary<string, object> jwk)
        {
            if (!jwk.ContainsKey(HttpSigningConstants.RS.ModulusProperty))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.RS.ModulusProperty);
            }
            if (!jwk.ContainsKey(HttpSigningConstants.RS.ExponentProperty))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.RS.ExponentProperty);
            }

            ModulusString = jwk[HttpSigningConstants.RS.ModulusProperty] as string;
            ExponentString = jwk[HttpSigningConstants.RS.ExponentProperty] as string;

            if (ModulusString == null)
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.RS.ModulusProperty);
            }
            if (ExponentString == null)
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.RS.ExponentProperty);
            }

            ModulusBytes = Base64Url.Decode(ModulusString);
            ExponentBytes = Base64Url.Decode(ExponentString);
        }

        public override Signature ToSignature()
        {
            switch (Jwk.Algorithm)
            {
                case "RS256": return new RS256Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
                case "RS384": return new RS384Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
                case "RS512": return new RS512Signature(new RSAParameters { Modulus = ModulusBytes, Exponent = ExponentBytes });
            }

            throw new InvalidOperationException("Invalid algorithm");
        }
    }
}
