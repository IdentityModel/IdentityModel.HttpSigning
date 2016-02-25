// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpSigning
{
    public class Signature
    {
        private readonly JwsAlgorithm _alg;
        private readonly object _key;

        protected Signature(object key, JwsAlgorithm alg)
        {
            if (key == null) throw new ArgumentNullException("key");

            _key = key;
            _alg = alg;
        }

        public string Sign(SigningParameters payload)
        {
            if (payload == null) throw new ArgumentNullException("payload");

            return JWT.Encode(payload.ToEncodedDictionary(), _key, _alg);
        }
    }

    public class RS256Signature : Signature
    {
        public RS256Signature(RSACryptoServiceProvider key)
            : base(key, JwsAlgorithm.RS256)
        {
        }
    }
    public class RS384Signature : Signature
    {
        public RS384Signature(RSACryptoServiceProvider key)
            : base(key, JwsAlgorithm.RS384)
        {
        }
    }
    public class RS512Signature : Signature
    {
        public RS512Signature(RSACryptoServiceProvider key)
            : base(key, JwsAlgorithm.RS512)
        {
        }
    }

    public class HS256Signature : Signature
    {
        public HS256Signature(byte[] key)
            : base(key, JwsAlgorithm.HS256)
        {
        }
    }
    public class HS384Signature : Signature
    {
        public HS384Signature(byte[] key)
            : base(key, JwsAlgorithm.HS384)
        {
        }
    }
    public class HS512Signature : Signature
    {
        public HS512Signature(byte[] key)
            : base(key, JwsAlgorithm.HS512)
        {
        }
    }
}
