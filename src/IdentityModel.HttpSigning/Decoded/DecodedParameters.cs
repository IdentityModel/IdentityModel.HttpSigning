// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IdentityModel.HttpSigning
{
    public class DecodedParameters
    {
        public DecodedParameters(IDictionary<string, object> values)
        {
            Decode(values);
        }

        public DecodedParameters(string json)
        {
            if (String.IsNullOrWhiteSpace(json)) throw new ArgumentNullException("json");

            Dictionary<string, object> values = null;
            try
            {
                values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            catch
            {
                throw new ArgumentException("Invalid JSON");
            }

            Decode(values);
        }

        public string AccessToken { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string Host { get; set; }
        public string UrlPath { get; set; }
        public DecodedList QueryParameters { get; set; }
        public DecodedList RequestHeaders { get; set; }
        public string BodyHash { get; set; }

        private void Decode(IDictionary<string, object> values)
        {
            if (values == null) throw new ArgumentNullException("values");

            if (values.ContainsKey(HttpSigningConstants.SignedObjectParameterNames.AccessToken) == false)
            {
                throw new ArgumentException(HttpSigningConstants.SignedObjectParameterNames.AccessToken + " value not present");
            }

            AccessToken = GetString(values, HttpSigningConstants.SignedObjectParameterNames.AccessToken);
            if (AccessToken == null) throw new ArgumentException(HttpSigningConstants.SignedObjectParameterNames.AccessToken + " value not present");

            var ts = GetNumber(values, HttpSigningConstants.SignedObjectParameterNames.TimeStamp);
            if (ts != null) TimeStamp = ts.Value.ToDateTimeOffsetFromEpoch();

            var method = GetString(values, HttpSigningConstants.SignedObjectParameterNames.HttpMethod);
            if (method != null) HttpMethod = new HttpMethod(method);

            Host = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Host);
            UrlPath = GetString(values, HttpSigningConstants.SignedObjectParameterNames.UrlPath);
            QueryParameters = GetDecodedList(values, HttpSigningConstants.SignedObjectParameterNames.HashedQueryParameters);
            RequestHeaders = GetDecodedList(values, HttpSigningConstants.SignedObjectParameterNames.HashedRequestHeaders);
            BodyHash = GetString(values, HttpSigningConstants.SignedObjectParameterNames.HashedRequestBody);
        }

        DecodedList GetDecodedList(IDictionary<string, object> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var item = values[key];
                return new DecodedList(item);
            }
            return null;
        }

        string GetString(IDictionary<string, object> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var item = values[key] as string;
                if (item == null)
                {
                    throw new ArgumentException(key + " must be a string");
                }
                return item;
            }
            return null;
        }

        long? GetNumber(IDictionary<string, object> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var item = values[key];
                var type = item.GetType();

                if (typeof(long) == type)
                {
                    return (long)item;
                }

                if (typeof(int) == type)
                {
                    return (int)item;
                }

                if (typeof(short) == type)
                {
                    return (short)item;
                }

                throw new ArgumentException(key + " must be a number");
            }
            return null;
        }
    }
}
