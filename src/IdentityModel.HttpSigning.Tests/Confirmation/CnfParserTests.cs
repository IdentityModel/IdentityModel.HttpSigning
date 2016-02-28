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
            Assert.Throws<ArgumentNullException>(() => CnfParser.Parse(null));
            Assert.ThrowsAny<Exception>(() => CnfParser.Parse("not jwk json"));
        }

        [Fact]
        public void parser_should_read_json()
        {
            var jwk = CnfParser.Parse("{}");
            jwk.Should().NotBeNull();
        }
    }
}
