// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Jose;
using System;
using System.Security.Cryptography;

namespace IdentityModel.HttpSigning
{
    static class RSAParametersExtensions
    {
        public static RSACryptoServiceProvider ToRSACryptoServiceProvider(this RSAParameters rsa)
        {
            var csp = new CspParameters();
            csp.Flags = CspProviderFlags.CreateEphemeralKey;
            csp.KeyNumber = (int)KeyNumber.Signature;

            var prov = new RSACryptoServiceProvider(2048, csp);
            prov.ImportParameters(rsa);

            return prov;
        }
    }

    public class RS256Signature : Signature
    {
        public RS256Signature(RSAParameters rsa)
            : base(JwsAlgorithm.RS256, rsa.ToRSACryptoServiceProvider())
        {
        }

        public RS256Signature(RSACryptoServiceProvider key)
            : base(JwsAlgorithm.RS256, key)
        {
        }
    }
    public class RS384Signature : Signature
    {
        public RS384Signature(RSAParameters rsa)
            : base(JwsAlgorithm.RS384, rsa.ToRSACryptoServiceProvider())
        {
        }

        public RS384Signature(RSACryptoServiceProvider key)
            : base(JwsAlgorithm.RS384, key)
        {
        }
    }
    public class RS512Signature : Signature
    {
        public RS512Signature(RSAParameters rsa)
            : base(JwsAlgorithm.RS512, rsa.ToRSACryptoServiceProvider())
        {
        }

        public RS512Signature(RSACryptoServiceProvider key)
            : base(JwsAlgorithm.RS512, key)
        {
        }
    }
}
