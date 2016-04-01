// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel.HttpSigning;
using IdentityModel.HttpSigning.Logging;
using System;

namespace IdentityModel.Jwt
{
    public static class JsonWebKeyExtensions
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public static SigningKey ToPublicKey(this JsonWebKey key)
        {
            if (key.Kty == HttpSigningConstants.Jwk.Symmetric.KeyType)
            {
                return new SymmetricKey(key);
            }

            if (key.Kty == HttpSigningConstants.Jwk.RSA.KeyType)
            {
                return new RSAPublicKey(key);
            }

            Logger.Error("Invalid key type: " + key.Kty);
            throw new InvalidOperationException("Invalid key type");
        }
    }
}
