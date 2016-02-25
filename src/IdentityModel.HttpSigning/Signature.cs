// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Jose;
using System;
using System.Security.Cryptography;

namespace IdentityModel.HttpSigning
{
    public class Signature
    {
        private readonly JwsAlgorithm _alg;
        private readonly object _key;

        protected Signature(JwsAlgorithm alg, object key = null)
        {
            _alg = alg;
            _key = key;
        }

        public string Sign(EncodingParameters payload)
        {
            if (payload == null) throw new ArgumentNullException("payload");

            return JWT.Encode(payload.ToEncodedDictionary(), _key, _alg);
        }

        public DecodedParameters Verify(string token)
        {
            if (token == null) throw new ArgumentNullException("token");

            try
            {
                var json = JWT.Decode(token, _key);
                if (json == null) return null;

                return new DecodedParameters(json);
            }
            catch
            {
                return null;
            }
        }
    }

    public class RS256Signature : Signature
    {
        public RS256Signature(RSACryptoServiceProvider key)
            : base(JwsAlgorithm.RS256, key)
        {
        }
    }
    public class RS384Signature : Signature
    {
        public RS384Signature(RSACryptoServiceProvider key)
            : base(JwsAlgorithm.RS384, key)
        {
        }
    }
    public class RS512Signature : Signature
    {
        public RS512Signature(RSACryptoServiceProvider key)
            : base(JwsAlgorithm.RS512, key)
        {
        }
    }

    public class HS256Signature : Signature
    {
        public HS256Signature(byte[] key)
            : base(JwsAlgorithm.HS256, key)
        {
        }
    }
    public class HS384Signature : Signature
    {
        public HS384Signature(byte[] key)
            : base(JwsAlgorithm.HS384, key)
        {
        }
    }
    public class HS512Signature : Signature
    {
        public HS512Signature(byte[] key)
            : base(JwsAlgorithm.HS512, key)
        {
        }
    }
}
