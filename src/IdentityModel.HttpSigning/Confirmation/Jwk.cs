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
    public class Jwk
    {
        public Jwk(IDictionary<string, object> jwk)
        {
            if (jwk == null) throw new ArgumentNullException("jwk");

            Contents = jwk;
            Read(jwk);
        }

        public IDictionary<string, object> Contents { get; private set; }

        void Read(IDictionary<string, object> jwk)
        {
            if (!jwk.ContainsKey(HttpSigningConstants.Jwk.KeyTypeProperty))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.KeyTypeProperty);
            }
            if (!jwk.ContainsKey(HttpSigningConstants.Jwk.AlgorithmProperty))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.AlgorithmProperty);
            }
            if (!jwk.ContainsKey(HttpSigningConstants.Jwk.KeyIdProperty))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.Jwk.KeyIdProperty);
            }

            KeyType = jwk[HttpSigningConstants.Jwk.KeyTypeProperty] as string;
            Algorithm = jwk[HttpSigningConstants.Jwk.AlgorithmProperty] as string;
            KeyId = jwk[HttpSigningConstants.Jwk.KeyIdProperty] as string;

            if (KeyType == null)
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.Jwk.KeyTypeProperty);
            }
            if (Algorithm == null)
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.Jwk.AlgorithmProperty);
            }
            if (KeyId == null)
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.Jwk.KeyIdProperty);
            }
        }

        public string KeyType { get; set; }
        public string Algorithm { get; set; }
        public string KeyId { get; set; }

        public PublicKey ToPublicKey()
        {
            if (KeyType.StartsWith("HS"))
            {
                return new HSPublicKey(this);
            }

            if (KeyType.StartsWith("RS"))
            {
                return new RSPublicKey(this);
            }

            throw new InvalidOperationException("Invalid key type");
        }
    }
}
