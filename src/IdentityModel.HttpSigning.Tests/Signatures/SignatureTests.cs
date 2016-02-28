// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Jose;

namespace IdentityModel.HttpSigning.Tests
{
    public class SignatureTests
    {
        static readonly byte[] _symmetricKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };
        static System.Security.Cryptography.RSACryptoServiceProvider _asymmetricKey;

        static SignatureTests()
        {
            var csp = new System.Security.Cryptography.CspParameters();
            csp.Flags = System.Security.Cryptography.CspProviderFlags.CreateEphemeralKey;
            csp.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Signature;
            _asymmetricKey = new System.Security.Cryptography.RSACryptoServiceProvider(2048, csp);
        }

        [Fact]
        public void symmetric_signed_result_should_be_able_to_verifed()
        {
            var subject = new HS256Signature(_symmetricKey);
            var encoding = new EncodingParameters("foo");

            var token = subject.Sign(encoding);
            var decoded = subject.Verify(token);

            decoded.AccessToken.Should().Be("foo");
        }

        [Fact]
        public void asymmetric_signed_result_should_be_able_to_verifed()
        {
            var subject = new RS256Signature(_asymmetricKey);
            var encoding = new EncodingParameters("foo");

            var token = subject.Sign(encoding);
            var decoded = subject.Verify(token);

            decoded.AccessToken.Should().Be("foo");
        }

        [Fact]
        public void when_using_wrong_key_should_not_be_able_to_verify()
        {
            var encoding = new EncodingParameters("foo");
            var token = new RS256Signature(_asymmetricKey).Sign(encoding);

            var subject = new HS256Signature(_symmetricKey);
            var decoded = subject.Verify(token);
            decoded.Should().BeNull();
        }
    }
}
