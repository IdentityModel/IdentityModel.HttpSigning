// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityModel.HttpSigning
{
    public class HttpSigningConstants
    {
        public class AccessTokenParameterNames
        {
            public const string AuthorizationHeaderScheme = "PoP";
            public const string RequestParameterName = "pop_access_token";
        }

        public class SignedObjectParameterNames
        {
            public const string AccessToken = "at";
            public const string TimeStamp = "ts";
            public const string HttpMethod = "m";
            public const string Host = "u";
            public const string Path = "p";
            public const string HashedQueryParameters = "q";
            public const string HashedRequestHeaders = "h";
            public const string HashedRequestBody = "b";
        }

        public class HashedQuerySeparators
        {
            public const string KeyValueSeparator = "=";
            public const string ParameterSeparator = "&";
        }

        public class HashedRequestHeaderSeparators
        {
            public const string KeyValueSeparator = ": ";
            public const string ParameterSeparator = "\n";
        }

        public class Confirmation
        {
            public const string ConfirmationProperty = "cnf";
            public const string JwkProperty = "jwk";
        }

        public class Jwk
        {
            public const string KeyTypeProperty = "kty";
            public const string AlgorithmProperty = "alg";
            public const string KeyIdProperty = "kid";

            public class Symmetric
            {
                public static readonly string[] Algorithms = new string[]
                {
                "HS256", "HS384", "HS512"
                };

                public const string KeyType = "oct";
                public const string KeyProperty = "k";
            }

            public class RSA
            {
                public static readonly string[] Algorithms = new string[]
                {
                "RS256", "RS384", "RS512"
                };
                public const string KeyType = "RSA";
                public const string ModulusProperty = "n";
                public const string ExponentProperty = "e";
            }
        }
    }
}
