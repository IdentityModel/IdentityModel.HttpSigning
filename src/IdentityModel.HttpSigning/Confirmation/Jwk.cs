// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel.HttpSigning.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpSigning
{
    public class Jwk
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public string kty { get; set; }
        public string alg { get; set; }
        public string kid { get; set; }

        // asymmetric modulus
        public string n { get; set; }
        // asymmetric exponent
        public string e { get; set; }
        
        // symmetric key
        public string k { get; set; }

        public SigningKey ToPublicKey()
        {
            if (kty == HttpSigningConstants.Jwk.Symmetric.KeyType)
            {
                return new SymmetricKey(this);
            }

            if (kty == HttpSigningConstants.Jwk.RSA.KeyType)
            {
                return new RSAPublicKey(this);
            }

            Logger.Error("Invalid key type: " + kty);
            throw new InvalidOperationException("Invalid key type");
        }
    }
}
