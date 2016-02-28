// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Jose;
using System;
using System.Security.Cryptography;

namespace IdentityModel.HttpSigning
{
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
