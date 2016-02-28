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
}
