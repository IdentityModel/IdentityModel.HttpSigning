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
    public class CnfParser
    {
        public CnfParser(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException("json");
            }

            Parse(json);
        }

        public Jwk Jwk { get; set; }

        void Parse(string json)
        {
            var cnf = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            if (!cnf.ContainsKey(HttpSigningConstants.Cnf.JwkProperty))
            {
                throw new ArgumentException("Missing " + HttpSigningConstants.Cnf.JwkProperty);
            }

            var jwk = cnf["jwk"] as Dictionary<string, object>;
            if (jwk == null)
            {
                throw new ArgumentException("Invalid " + HttpSigningConstants.Cnf.JwkProperty);
            }

            Jwk = new Jwk(jwk);
        }
    }
}
