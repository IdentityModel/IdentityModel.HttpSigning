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

namespace IdentityModel.HttpSigning.Tests.Confirmation
{
    public class CnfParserTests
    {
        [Fact]
        public void requires_json()
        {
            CnfParser.Parse(null).Should().BeNull();
            CnfParser.Parse("not jwk json").Should().BeNull();
        }

        [Fact]
        public void parser_should_return_cnf_property_from_json()
        {
            var jwk = CnfParser.Parse("{}");
            jwk.Should().BeNull();
        }

        [Fact]
        public void parser_should_read_proper_cnf_structure()
        {
            var jwk = new Jwk
            {
                kty = "oct",
                alg = "HS256",
                k = "123"
            };

            var json = new Cnf(jwk).ToJson();
            jwk = CnfParser.Parse(json);
            jwk.Should().NotBeNull();
            jwk.kty.Should().Be("oct");
            jwk.alg.Should().Be("HS256");
            jwk.k.Should().Be("123");
        }
    }
}
