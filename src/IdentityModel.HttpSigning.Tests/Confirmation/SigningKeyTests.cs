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
    public class SigningKeyTests
    {
        [Fact]
        public void SymmetricKey_should_require_valid_key_values()
        {
            Assert.Throws<ArgumentException>(() => new SymmetricKey(new Jwk()));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new Jwk()
            {
                alg = "HS256",
                k = "not a base64url encoded value"
            }));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new Jwk()
            {
                alg = "not valid",
                k = "1234"
            }));
        }

        [Fact]
        public void SymmetricKey_should_allow_all_algs()
        {
            var jwk = new Jwk()
            {
                k = "1234"
            };

            jwk.alg = "HS256";
            new SymmetricKey(jwk).KeyBytes.Should().NotBeNull();

            jwk.alg = "HS384";
            new SymmetricKey(jwk).KeyBytes.Should().NotBeNull();

            jwk.alg = "HS512";
            new SymmetricKey(jwk).KeyBytes.Should().NotBeNull();
        }

        [Fact]
        public void ToSignature_should_create_correct_signature()
        {
            var jwk = new Jwk()
            {
                k = "1234"
            };

            jwk.alg = "HS256";
            new SymmetricKey(jwk).ToSignature().Should().BeOfType<HS256Signature>();

            jwk.alg = "HS384";
            new SymmetricKey(jwk).ToSignature().Should().BeOfType<HS384Signature>();

            jwk.alg = "HS512";
            new SymmetricKey(jwk).ToSignature().Should().BeOfType<HS512Signature>();
        }

        [Fact]
        public void RSAKey_should_require_valid_key_values()
        {
            Assert.Throws<ArgumentException>(() => new SymmetricKey(new Jwk()));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new Jwk()
            {
                alg = "not a valuid alg",
                e = "1234",
                n = "1234"
            }));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new Jwk()
            {
                alg = "RS256",
                e = "not a base64url encoded value",
                n = "1234"
            }));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new Jwk()
            {
                alg = "RS256",
                n = "not a base64url encoded value",
                e = "1234"
            }));
        }

        [Fact]
        public void RSAKey_should_allow_all_algs()
        {
            var jwk = new Jwk()
            {
                n = "1234",
                e = "1234"
            };

            jwk.alg = "RS256";
            new RSASigningKey(jwk).ModulusBytes.Should().NotBeNull();

            jwk.alg = "RS384";
            new RSASigningKey(jwk).ModulusBytes.Should().NotBeNull();

            jwk.alg = "RS512";
            new RSASigningKey(jwk).ModulusBytes.Should().NotBeNull();
        }

        [Fact]
        public void RSAKey_should_create_correct_signature()
        {
            var jwk = new Jwk()
            {
                n = "0vx7agoebGcQSuuPiLJXZptN9nndrQmbXEps2aiAFbWhM78LhWx4cbbfAAtVT86zwu1RK7aPFFxuhDR1L6tSoc_BJECPebWKRXjBZCiFV4n3oknjhMstn64tZ_2W-5JsGY4Hc5n9yBXArwl93lqt7_RN5w6Cf0h4QyQ5v-65YGjQR0_FDW2QvzqY368QQMicAtaSqzs8KJZgnYb9c7d0zgdAZHzu6qMQvRL5hajrn1n91CbOpbISD08qNLyrdkt-bFTWhAI4vMQFh6WeZu0fM4lFd2NcRwr3XPksINHaQ-G_xBniIqbw0Ls1jF44-csFCur-kEgU8awapJzKnqDKgw",
                e = "AQAB"
            };

            jwk.alg = "RS256";
            new RSASigningKey(jwk).ToSignature().Should().BeOfType<RS256Signature>();

            jwk.alg = "RS384";
            new RSASigningKey(jwk).ToSignature().Should().BeOfType<RS384Signature>();

            jwk.alg = "RS512";
            new RSASigningKey(jwk).ToSignature().Should().BeOfType<RS512Signature>();
        }
    }
}
