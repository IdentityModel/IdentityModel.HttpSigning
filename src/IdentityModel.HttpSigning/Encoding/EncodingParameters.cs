// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;

namespace IdentityModel.HttpSigning
{
    public class EncodingParameters
    {
        public EncodingParameters(string accessToken)
        {
            if (String.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            AccessToken = accessToken;
            TimeStamp = DateTimeOffset.UtcNow;
            QueryParameters = new List<KeyValuePair<string, string>>();
            RequestHeaders = new List<KeyValuePair<string, string>>();
        }

        public string AccessToken { get; private set; }
        public DateTimeOffset TimeStamp { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public IList<KeyValuePair<string, string>> QueryParameters { get; set; }
        public IList<KeyValuePair<string, string>> RequestHeaders { get; set; }
        public byte[] Body { get; set; }

        public EncodedParameters Encode()
        {
            var result = new EncodedParameters(AccessToken);
            result.TimeStamp = TimeStamp.ToEpochTime();

            if (HttpMethod != null)
            {
                result.HttpMethod = HttpMethod.Method;
            }
            result.Host = Host;
            result.Path = Path;   

            if (QueryParameters != null && QueryParameters.Any())
            {
                var query = new EncodingQueryParameters(QueryParameters);
                result.QueryParameters = query.Encode();
            }

            if (RequestHeaders != null && RequestHeaders.Any())
            {
                var headers = new EncodingHeaderList(RequestHeaders);
                result.RequestHeaders = headers.Encode();
            }

            if (Body != null)
            {
                result.BodyHash = CalculateBodyHash();
            }

            return result;
        }

        string CalculateBodyHash()
        {
            var hash = SHA256.Create().ComputeHash(Body);
            return Jose.Base64Url.Encode(hash);
        }
    }
}
