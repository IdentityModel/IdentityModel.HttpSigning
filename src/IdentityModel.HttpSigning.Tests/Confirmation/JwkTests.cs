// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using IdentityModel.Jwt;

namespace IdentityModel.HttpSigning.Tests.Confirmation
{
    public class JwkTests
    {
        [Fact]
        public void invalid_key_type_should_throw()
        {
            var jwk = new JsonWebKey
            {
                Kty = "foo",
                K = "1234",
                E = "1234",
                N = "abc"
            };

            Assert.Throws<InvalidOperationException>(() => jwk.ToPublicKey());
        }

        [Fact]
        public void symmetric_key_type_should_create_correct_public_key_object()
        {
            var jwk = new JsonWebKey
            {
                Kty = "oct",
                Alg = "HS256",
                K = "1234"
            };

            var key = jwk.ToPublicKey();
            key.Should().BeOfType<SymmetricKey>();
        }

        [Fact]
        public void asymmetric_key_type_should_create_correct_public_key_object()
        {
            var jwk = new JsonWebKey
            {
                Kty = "RSA",
                Alg = "RS256",
                E = "1234",
                N = "abc"
            };

            var key = jwk.ToPublicKey();
            key.Should().BeOfType<RSAPublicKey>();
        }
    }
}
