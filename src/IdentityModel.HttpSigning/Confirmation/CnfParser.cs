// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel.HttpSigning.Logging;
using IdentityModel.Jwt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpSigning
{
    public class Cnf
    {
        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public Cnf(JsonWebKey jwk)
        {
            this.jwk = jwk;
        }

        public JsonWebKey jwk { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, _jsonSettings);
        }

      
    }

    public class CnfParser
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static JsonWebKey Parse(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            Cnf cnf = null;
            try
            {
                cnf = JsonConvert.DeserializeObject<Cnf>(json, _jsonSettings);
            }
            catch(Exception ex)
            {
                Logger.ErrorException("Failed to parse CNF JSON", ex);
            }

            if (cnf == null)
            {
                Logger.Error("cnf JSON failed to parse");
            }
            else if (cnf.jwk == null)
            {
                Logger.Error("jwk missing in cnf");
            }

            return cnf?.jwk;
        }
    }
}
