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
    public class SigningKeyTests
    {
        [Fact]
        public void SymmetricKey_should_require_valid_key_values()
        {
            Assert.Throws<ArgumentException>(() => new SymmetricKey(new JsonWebKey()));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new JsonWebKey()
            {
                Alg = "HS256",
                K = "not a base64url encoded value"
            }));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new JsonWebKey()
            {
                Alg = "not valid",
                K = "1234"
            }));
        }

        [Fact]
        public void SymmetricKey_should_allow_all_algs()
        {
            var jwk = new JsonWebKey()
            {
                K = "1234"
            };

            jwk.Alg = "HS256";
            new SymmetricKey(jwk).KeyBytes.Should().NotBeNull();

            jwk.Alg = "HS384";
            new SymmetricKey(jwk).KeyBytes.Should().NotBeNull();

            jwk.Alg = "HS512";
            new SymmetricKey(jwk).KeyBytes.Should().NotBeNull();
        }

        [Fact]
        public void ToSignature_should_create_correct_signature()
        {
            var jwk = new JsonWebKey()
            {
                K = "1234"
            };

            jwk.Alg = "HS256";
            new SymmetricKey(jwk).ToSignature().Should().BeOfType<HS256Signature>();

            jwk.Alg = "HS384";
            new SymmetricKey(jwk).ToSignature().Should().BeOfType<HS384Signature>();

            jwk.Alg = "HS512";
            new SymmetricKey(jwk).ToSignature().Should().BeOfType<HS512Signature>();
        }

        [Fact]
        public void RSAKey_should_require_valid_key_values()
        {
            Assert.Throws<ArgumentException>(() => new SymmetricKey(new JsonWebKey()));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new JsonWebKey()
            {
                Alg = "not a valuid alg",
                E = "1234",
                N = "1234"
            }));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new JsonWebKey()
            {
                Alg = "RS256",
                E = "not a base64url encoded value",
                N = "1234"
            }));
            Assert.ThrowsAny<Exception>(() => new SymmetricKey(new JsonWebKey()
            {
                Alg = "RS256",
                N = "not a base64url encoded value",
                E = "1234"
            }));
        }

        [Fact]
        public void RSAKey_should_allow_all_algs()
        {
            var jwk = new JsonWebKey()
            {
                N = "1234",
                E = "1234"
            };

            jwk.Alg = "RS256";
            new RSAPublicKey(jwk).ModulusBytes.Should().NotBeNull();

            jwk.Alg = "RS384";
            new RSAPublicKey(jwk).ModulusBytes.Should().NotBeNull();

            jwk.Alg = "RS512";
            new RSAPublicKey(jwk).ModulusBytes.Should().NotBeNull();
        }

        [Fact]
        public void RSAKey_should_create_correct_signature()
        {
            var jwk = new JsonWebKey()
            {
                N = "0vx7agoebGcQSuuPiLJXZptN9nndrQmbXEps2aiAFbWhM78LhWx4cbbfAAtVT86zwu1RK7aPFFxuhDR1L6tSoc_BJECPebWKRXjBZCiFV4n3oknjhMstn64tZ_2W-5JsGY4Hc5n9yBXArwl93lqt7_RN5w6Cf0h4QyQ5v-65YGjQR0_FDW2QvzqY368QQMicAtaSqzs8KJZgnYb9c7d0zgdAZHzu6qMQvRL5hajrn1n91CbOpbISD08qNLyrdkt-bFTWhAI4vMQFh6WeZu0fM4lFd2NcRwr3XPksINHaQ-G_xBniIqbw0Ls1jF44-csFCur-kEgU8awapJzKnqDKgw",
                E = "AQAB"
            };

            jwk.Alg = "RS256";
            new RSAPublicKey(jwk).ToSignature().Should().BeOfType<RS256Signature>();

            jwk.Alg = "RS384";
            new RSAPublicKey(jwk).ToSignature().Should().BeOfType<RS384Signature>();

            jwk.Alg = "RS512";
            new RSAPublicKey(jwk).ToSignature().Should().BeOfType<RS512Signature>();
        }
    }
}
