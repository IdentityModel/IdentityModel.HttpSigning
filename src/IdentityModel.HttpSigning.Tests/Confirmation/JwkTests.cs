// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace IdentityModel.HttpSigning.Tests.Confirmation
{
    public class JwkTests
    {
        [Fact]
        public void invalid_key_type_should_throw()
        {
            var jwk = new Jwk
            {
                kty = "foo",
                k = "1234",
                e = "1234",
                n = "abc"
            };

            Assert.Throws<InvalidOperationException>(() => jwk.ToPublicKey());
        }

        [Fact]
        public void symmetric_key_type_should_create_correct_public_key_object()
        {
            var jwk = new Jwk
            {
                kty = "oct",
                alg = "HS256",
                k = "1234"
            };

            var key = jwk.ToPublicKey();
            key.Should().BeOfType<SymmetricKey>();
        }

        [Fact]
        public void asymmetric_key_type_should_create_correct_public_key_object()
        {
            var jwk = new Jwk
            {
                kty = "RSA",
                alg = "RS256",
                e = "1234",
                n = "abc"
            };

            var key = jwk.ToPublicKey();
            key.Should().BeOfType<RSAPublicKey>();
        }
    }
}
