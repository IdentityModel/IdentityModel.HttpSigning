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

            var encodedPayload = payload.Encode();
            return JWT.Encode(encodedPayload.Encode(), _key, _alg);
        }

        public EncodedParameters Verify(string token)
        {
            if (token == null) throw new ArgumentNullException("token");

            try
            {
                var json = JWT.Decode(token, _key);
                if (json == null) return null;

                return EncodedParameters.FromJson(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
