// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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

        public Cnf(Jwk jwk)
        {
            this.jwk = jwk;
        }

        public Jwk jwk { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, _jsonSettings);
        }
    }

    public class CnfParser
    {
        static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static Jwk Parse(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException("json");
            }

            var cnf = JsonConvert.DeserializeObject<Cnf>(json, _jsonSettings);
            return cnf?.jwk;
        }
    }
}
