// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel.HttpSigning.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IdentityModel.HttpSigning
{
    public class EncodedParameters
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

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

        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static EncodedParameters FromJson(string json)
        {
            if (String.IsNullOrWhiteSpace(json)) throw new ArgumentNullException("json");

            Dictionary<string, object> values = null;
            try
            {
                values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, _jsonSettings);
            }
            catch(Exception ex)
            {
                Logger.ErrorException("Failed to deserialize JSON", ex);
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

            if (AccessToken != other.AccessToken)
            {
                Logger.Debug("AccessToken mismatch");
                return false;
            }
            if (Method != other.Method)
            {
                Logger.Debug("Method mismatch");
                return false;
            }
            if (Host != other.Host)
            {
                Logger.Debug("Host mismatch");
                return false;
            }
            if (Path != other.Path)
            {
                Logger.Debug("Path mismatch");
                return false;
            }
            if (BodyHash != other.BodyHash)
            {
                Logger.Debug("BodyHash mismatch");
                return false;
            }

            if (QueryParameters == null && other.QueryParameters != null)
            {
                Logger.Debug("One QueryParameters is null, the other is not");
                return false;
            }
            if (QueryParameters != null && other.QueryParameters == null)
            {
                Logger.Debug("One QueryParameters is null, the other is not");
                return false;
            }
            if (QueryParameters != null && !QueryParameters.IsSame(other.QueryParameters))
            {
                Logger.Debug("QueryParameters mismatch");
                return false;
            }

            if (RequestHeaders == null && other.RequestHeaders != null)
            {
                Logger.Debug("One RequestHeaders is null, the other is not");
                return false;
            }
            if (RequestHeaders != null && other.RequestHeaders == null)
            {
                Logger.Debug("One RequestHeaders is null, the other is not");
                return false;
            }
            if (RequestHeaders != null && !RequestHeaders.IsSame(other.RequestHeaders))
            {
                Logger.Debug("RequestHeaders mismatch");
                return false;
            }

            return true;
        }

        public Dictionary<string, object> Encode()
        {
            var value = new Dictionary<string, object>();

            value.Add(HttpSigningConstants.SignedObjectParameterNames.AccessToken, AccessToken);

            if (TimeStamp != null)
            {
                Logger.Debug("Encoding timestamp");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.TimeStamp, TimeStamp.Value);
            }

            if (Method != null)
            {
                Logger.Debug("Encoding method");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Method, Method);
            }

            if (Host != null)
            {
                Logger.Debug("Encoding host");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Host, Host);
            }

            if (Path != null)
            {
                Logger.Debug("Encoding path");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.Path, Path);
            }

            if (QueryParameters != null)
            {
                Logger.Debug("Encoding query params");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedQueryParameters, QueryParameters.Encode());
            }

            if (RequestHeaders != null)
            {
                Logger.Debug("Encoding request headers");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedRequestHeaders, RequestHeaders.Encode());
            }

            if (BodyHash != null)
            {
                Logger.Debug("Encoding body hash");
                value.Add(HttpSigningConstants.SignedObjectParameterNames.HashedRequestBody, BodyHash);
            }

            return value;
        }

        private void Decode(IDictionary<string, object> values)
        {
            AccessToken = GetString(values, HttpSigningConstants.SignedObjectParameterNames.AccessToken);
            if (AccessToken == null)
            {
                Logger.Error(HttpSigningConstants.SignedObjectParameterNames.AccessToken + " value not present");
                throw new ArgumentException(HttpSigningConstants.SignedObjectParameterNames.AccessToken + " value not present");
            }

            var ts = GetNumber(values, HttpSigningConstants.SignedObjectParameterNames.TimeStamp);
            if (ts != null)
            {
                Logger.Debug("Decoded Timestamp");
                TimeStamp = ts;
            }

            Method = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Method);
            if (Method != null) Logger.Debug("Decoded Method");

            Host = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Host);
            if (Host != null) Logger.Debug("Decoded Host");

            Path = GetString(values, HttpSigningConstants.SignedObjectParameterNames.Path);
            if (Path != null) Logger.Debug("Decoded Path");

            QueryParameters = GetDecodedList(values, HttpSigningConstants.SignedObjectParameterNames.HashedQueryParameters);
            if (QueryParameters != null) Logger.Debug("Decoded QueryParameters");

            RequestHeaders = GetDecodedList(values, HttpSigningConstants.SignedObjectParameterNames.HashedRequestHeaders);
            if (RequestHeaders != null) Logger.Debug("Decoded RequestHeaders");

            BodyHash = GetString(values, HttpSigningConstants.SignedObjectParameterNames.HashedRequestBody);
            if (BodyHash != null) Logger.Debug("Decoded BodyHash");
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
                    Logger.Error(key + " must be a string");
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

                Logger.Error(key + " must be a number");
                throw new ArgumentException(key + " must be a number");
            }
            return null;
        }
    }
}
