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
            public const string UrlPath = "p";
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
    }
}
