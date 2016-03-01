// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using IdentityModel.HttpSigning;
using System.Net.Http;
using System.Security.Cryptography;

namespace IdentityModel.HttpSigning.Tests
{
    public class EncodingParametersTests
    {
        EncodingParameters _subject = new EncodingParameters("abc");

        [Fact]
        public void ctor_should_require_accesstoken()
        {
            Assert.Throws<ArgumentNullException>(() => new EncodingParameters(null));
            Assert.Throws<ArgumentNullException>(() => new EncodingParameters(""));
        }

        [Fact]
        public void encode_should_capture_access_token_and_timestamp()
        {
            var subject = new EncodingParameters("token");
            subject.AccessToken.Should().Be("token");

            var now = DateTimeOffset.UtcNow;
            subject.TimeStamp = now;

            var result = subject.Encode();
            result.AccessToken.Should().Be("token");
            result.TimeStamp.Should().Be(now.ToEpochTime());
            result.Method.Should().BeNull();
            result.Host.Should().BeNull();
            result.Path.Should().BeNull();
            result.QueryParameters.Should().BeNull();
            result.RequestHeaders.Should().BeNull();
            result.BodyHash.Should().BeNull();
        }

        [Fact]
        public void encode_should_capture_method()
        {
            var subject = new EncodingParameters("token");
            subject.Method = HttpMethod.Put;

            var result = subject.Encode();
            result.Method.Should().Be("PUT");
        }

        [Fact]
        public void encode_should_capture_host()
        {
            var subject = new EncodingParameters("token");
            subject.Host = "foo.com";

            var result = subject.Encode();
            result.Host.Should().Be("foo.com");
        }

        [Fact]
        public void encode_should_capture_path()
        {
            var subject = new EncodingParameters("token");
            subject.Path = "/path";

            var result = subject.Encode();
            result.Path.Should().Be("/path");
        }

        [Fact]
        public void encode_should_capture_query()
        {
            var subject = new EncodingParameters("token");
            subject.QueryParameters.Add(new KeyValuePair<string, string>("foo", "bar1"));
            subject.QueryParameters.Add(new KeyValuePair<string, string>("foo", "bar2"));

            var result = subject.Encode();
            result.QueryParameters.Should().NotBeNull();
            result.QueryParameters.Keys.Should().ContainInOrder(new string[] { "foo", "foo" });
            result.QueryParameters.HashedValue.Should().Be("EgePUeakH8URmSH3yz8zE39c0r4kYVYsuQRSZa_MdvQ");
        }

        [Fact]
        public void encode_should_capture_headers()
        {
            var subject = new EncodingParameters("token");
            subject.RequestHeaders.Add(new KeyValuePair<string, string>("foo", "bar1"));
            subject.RequestHeaders.Add(new KeyValuePair<string, string>("foo", "bar2"));

            var result = subject.Encode();
            result.RequestHeaders.Should().NotBeNull();
            result.RequestHeaders.Keys.Should().ContainInOrder(new string[] { "foo", "foo" });
            result.RequestHeaders.HashedValue.Should().Be("trcKLrzChz2G8_T50KpExyZ9DTfcsMTXsGGd3YwftLc");
        }

        [Fact]
        public void encode_should_capture_body()
        {
            var subject = new EncodingParameters("token");
            subject.Body = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            var result = subject.Encode();
            result.BodyHash.Should().Be("vnPfBFvKBXMv9S6m1FMSeSi1VLnnmqYXGr4xk9ImCp8");
        }

    }
}
