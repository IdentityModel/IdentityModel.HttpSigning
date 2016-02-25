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

namespace IdentityModel.HttpSigning.Tests
{
    public class DecodedParametersTests
    {
        [Fact]
        public void constructor_should_require_param()
        {
            var values = new Dictionary<string, object>();

            Assert.Throws<ArgumentNullException>(() => new DecodedParameters((string)null));
            Assert.Throws<ArgumentNullException>(() => new DecodedParameters((IDictionary<string, object>)null));
        }

        [Fact]
        public void constructor_should_require_json()
        {
            var values = new Dictionary<string, object>();

            Assert.Throws<ArgumentException>(() => new DecodedParameters("not json"));
        }

        [Fact]
        public void decoding_should_require_access_token()
        {
            var values = new Dictionary<string, object>();
            Assert.Throws<ArgumentException>(() => new DecodedParameters(values));
        }

        [Fact]
        public void decoding_should_require_access_token_to_be_a_string()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", 5 }
            };
            Assert.Throws<ArgumentException>(() => new DecodedParameters(values));
        }

        [Fact]
        public void decoding_should_parse_access_token()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" }
            };
            var subject = new DecodedParameters(values);
            subject.AccessToken.Should().Be("token");
        }

        [Fact]
        public void decoding_should_parse_timestamp()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "ts", 5000 }
            };
            var subject = new DecodedParameters(values);
            subject.TimeStamp.Should().HaveValue();
            subject.TimeStamp.Value.ToEpochTime().Should().Be(5000);
        }

        [Fact]
        public void decoding_should_require_timestamp_to_be_a_number()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "ts", "5000" }
            };

            Assert.Throws<ArgumentException>(() => new DecodedParameters(values));
        }

        [Fact]
        public void decoding_should_parse_http_method()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "m", "PUT" }
            };
            var subject = new DecodedParameters(values);
            subject.HttpMethod.Should().Be(HttpMethod.Put);
        }

        [Fact]
        public void decoding_should_parse_host()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "u", "foo.com:123" }
            };
            var subject = new DecodedParameters(values);
            subject.Host.Should().Be("foo.com:123");
        }

        [Fact]
        public void decoding_should_parse_url_path()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "p", "/foo/bar" }
            };
            var subject = new DecodedParameters(values);
            subject.UrlPath.Should().Be("/foo/bar");
        }

        [Fact]
        public void decoding_should_parse_query_params()
        {
            var list = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("a", "apple"),
                new KeyValuePair<string, string>("b", "banana"),
                new KeyValuePair<string, string>("c", "carrot"),
            };
            var query = new EncodingQueryParameters(list);
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "q",  query.ToEncodedArray()}
            };

            var subject = new DecodedParameters(values);
            subject.QueryParameters.Should().NotBeNull();
        }

        [Fact]
        public void decoding_should_parse_request_headers()
        {
            var list = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("a", "apple"),
                new KeyValuePair<string, string>("b", "banana"),
                new KeyValuePair<string, string>("c", "carrot"),
            };
            var query = new EncodingHeaderList(list);
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "h",  query.ToEncodedArray()}
            };
            var subject = new DecodedParameters(values);
            subject.RequestHeaders.Should().NotBeNull();
        }

        [Fact]
        public void decoding_should_parse_body()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "b", "body" }
            };
            var subject = new DecodedParameters(values);
            subject.BodyHash.Should().Be("body");
        }
    }
}
