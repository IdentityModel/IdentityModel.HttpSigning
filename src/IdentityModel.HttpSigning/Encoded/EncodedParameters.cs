// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IdentityModel.HttpSigning
{
    public class EncodedParameters
    {
        public EncodedParameters(string accessToken)
        {
            if (String.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            AccessToken = accessToken;
        }

        public EncodedParameters(IDictionary<string, object> values)
        {
            if (values == null) throw new ArgumentNullException("values");

            Decode(values);
        }

        public static EncodedParameters FromJson(string json)
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

            return new EncodedParameters(values);
        }

        public string AccessToken { get; private set; }
        public long? TimeStamp { get; set; }
        public string Method { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public EncodedList QueryParameters { get; set; }
        public EncodedList RequestHeaders { get; set; }
        public string BodyHash { get; set; }

        public bool IsSame(EncodedParameters other)
        {
            if (other == null) return false;

            if (AccessToken != other.AccessToken) return false;
            if (Method != other.Method) return false;
            if (Host != other.Host) return false;
            if (Path != other.Path) return false;
            if (BodyHash != other.BodyHash) return false;

            if (QueryParameters == null && other.QueryParameters != null) return false;
            if (QueryParameters != null && other.QueryParameters == null) return false;
            if (QueryParameters != null && !QueryParameters.IsSame(other.QueryParameters)) return false;

            if (RequestHeaders == null && other.RequestHeaders != null) return false;
            if (RequestHeaders != null && other.RequestHeaders == null) return false;
            if (RequestHeaders != null && !RequestHeaders.IsSame(other.RequestHeaders)) return false;

            return true;
        }

        public Dictionary<string, object> Encode()
        {
            var value = new Dictionary<string, object>();

            value.Add(HttpSigningConstants.SignedObjectParameterNames.AccessToken, AccessToken);

            if (TimeStamp != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.TimeStamp, TimeStamp.Value);
            }

            if (Method != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Method, Method);
            }

            if (Host != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Host, Host);
            }

            if (Path != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Path, Path);
            }

            if (QueryParameters != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedQueryParameters, QueryParameters.Encode());
            }

            if (RequestHeaders != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedRequestHeaders, RequestHeaders.Encode());
            }

            if (BodyHash != null)
            {
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedRequestBody, BodyHash);
            }

            return value;
        }

        private void Decode(IDictionary<string, object> values)
        {
            AccessToken = GetString(values, HttpSigningConstants.SignedObjectParameterNames.AccessToken);
            if (AccessToken == null) throw new ArgumentException(HttpSigningConstants.SignedObjectParameterNames.AccessToken + " value not present");

            var ts = GetNumber(values, HttpSigningConstants.SignedObjectParameterNames.TimeStamp);
            if (ts != null) TimeStamp = ts;

            Method = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Method);
            Host = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Host);
            Path = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Path);
            QueryParameters = GetDecodedList(values, HttpSigningConstants.SignedObjectParameterNames.HashedQueryParameters);
            RequestHeaders = GetDecodedList(values, HttpSigningConstants.SignedObjectParameterNames.HashedRequestHeaders);
            BodyHash = GetString(values, HttpSigningConstants.SignedObjectParameterNames.HashedRequestBody);
        }

        EncodedList GetDecodedList(IDictionary<string, object> values, string key)
        {
            if (values.ContainsKey(key))
            {
                var item = values[key];
                return new EncodedList(item);
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
