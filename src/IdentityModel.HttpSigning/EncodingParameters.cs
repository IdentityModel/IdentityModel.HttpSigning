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
        public EncodingParameters()
        {
            TimeStamp = DateTimeOffset.UtcNow;
            QueryParameters = new Dictionary<string, string>();
            RequestHeaders = new Dictionary<string, string>();
        }

        public string AccessToken { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string Host { get; set; }
        public string UrlPath { get; set; }
        public ICollection<KeyValuePair<string, string>> QueryParameters { get; set; }
        public ICollection<KeyValuePair<string, string>> RequestHeaders { get; set; }
        public byte[] Body { get; set; }

        public Dictionary<string, object> ToEncodedDictionary()
        {
            if (AccessToken == null) throw new InvalidOperationException("AccessToken is required");

            var value = new Dictionary<string, object>();

            value.Add(HttpSigningConstants.SignedObjectParameterNames.AccessToken, AccessToken);
            value.Add(HttpSigningConstants.SignedObjectParameterNames.TimeStamp, TimeStamp.ToEpochTime());

            if (HttpMethod != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HttpMethod, HttpMethod.Method);
            }

            if (Host != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Host, Host);
            }

            if (UrlPath != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.UrlPath, UrlPath);
            }

            if (QueryParameters != null && QueryParameters.Any())
            {
                var query = new EncodingQueryParameters(QueryParameters);
                var array = query.ToEncodedArray();
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedQueryParameters, array);
            }

            if (RequestHeaders != null && RequestHeaders.Any())
            {
                var headers = new EncodingHeaderList(RequestHeaders);
                var array = headers.ToEncodedArray();
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedRequestHeaders, array);
            }

            if (Body != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedRequestBody, CalculateBodyHash());
            }

            return value;
        }

        string CalculateBodyHash()
        {
            var hash = SHA256.Create().ComputeHash(Body);
            return Jose.Base64Url.Encode(hash);
        }
    }
}
