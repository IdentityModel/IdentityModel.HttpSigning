// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json;

namespace IdentityModel.HttpSigning.Tests.Integration
{
    public class IntegrationTests
    {
        static readonly byte[] _symmetricKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };

        [Fact]
        public void round_trip_path_should_validate()
        {
            Signature signature = new HS256Signature(_symmetricKey);

            var payload = new EncodingParameters("token");
            payload.Path = "/path";

            var token = signature.Sign(payload);

            var cnfJson = new Cnf(
                new Jwk
                {
                    kty = "oct",
                    alg = "HS256",
                    k = Base64Url.Encode(_symmetricKey)
                }).ToJson();
            var jwk = CnfParser.Parse(cnfJson);
            var key = jwk.ToPublicKey();
            signature = key.ToSignature();

            var result = signature.Verify(token);
            result.Should().NotBeNull();
            payload.Encode().IsSame(result).Should().BeTrue();
        }

        [Fact]
        public void round_trip_query_should_validate()
        {
            Signature signature = new HS256Signature(_symmetricKey);

            var payload = new EncodingParameters("token");
            payload.QueryParameters.Add(new KeyValuePair<string, string>("x", "1"));
            payload.QueryParameters.Add(new KeyValuePair<string, string>("y", "2"));

            var token = signature.Sign(payload);

            var cnfJson = new Cnf(
               new Jwk
               {
                   kty = "oct",
                   alg = "HS256",
                   k = Base64Url.Encode(_symmetricKey)
               }).ToJson();
            var jwk = CnfParser.Parse(cnfJson);
            var key = jwk.ToPublicKey();
            signature = key.ToSignature();

            var result = signature.Verify(token);
            result.Should().NotBeNull();
            payload.Encode().IsSame(result).Should().BeTrue();
        }
    }
}
