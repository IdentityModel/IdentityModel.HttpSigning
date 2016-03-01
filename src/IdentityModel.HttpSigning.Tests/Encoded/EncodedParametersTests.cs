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
    public class EncodedParametersTests
    {
        [Fact]
        public void constructor_should_require_param()
        {
            var values = new Dictionary<string, object>();

            Assert.Throws<ArgumentNullException>(() => new EncodedParameters((string)null));
            Assert.Throws<ArgumentNullException>(() => new EncodedParameters(""));
            Assert.Throws<ArgumentNullException>(() => new EncodedParameters((IDictionary<string, object>)null));
        }

        [Fact]
        public void encoding_should_contain_basic_values()
        {
            var subject = new EncodedParameters("abc");
            subject.TimeStamp = 456;

            var values = subject.Encode();

            values.Keys.Count.Should().Be(2);
            values.Should().ContainKey("at");
            values["at"].Should().Be("abc");
            values.Should().ContainKey("ts");
            ((long)values["ts"]).Should().Be(456);
        }

        [Fact]
        public void encoding_should_emit_http_method()
        {
            var subject = new EncodedParameters("abc");
            subject.HttpMethod = "POST";

            var values = subject.Encode();

            values.Should().ContainKey("m");
            values["m"].Should().Be("POST");
        }

        [Fact]
        public void encoding_should_emit_host()
        {
            var subject = new EncodedParameters("abc");
            subject.Host = "localhost:12345";

            var values = subject.Encode();

            values.Should().ContainKey("u");
            values["u"].Should().Be("localhost:12345");
        }

        [Fact]
        public void encoding_should_emit_url_path()
        {
            var subject = new EncodedParameters("abc");
            subject.Path = "/foo";

            var values = subject.Encode();

            values.Should().ContainKey("p");
            values["p"].Should().Be("/foo");
        }

        [Fact]
        public void encoding_should_emit_query_params()
        {
            var subject = new EncodedParameters("abc");
            var list = new EncodedList(new string[] { "foo", "foo" }, "hash");
            subject.QueryParameters = list;

            var values = subject.Encode();

            values.Should().ContainKey("q");
            values["q"].Should().BeAssignableTo<object[]>();
            var parts = (object[])values["q"];
            var keys = (IEnumerable<string>)parts[0];
            keys.Count().Should().Be(2);
            keys.Should().ContainInOrder(new string[] { "foo", "foo" });
            var value = (string)parts[1];
            value.Should().Be("hash");
        }

        [Fact]
        public void encoding_should_emit_header_list()
        {
            var subject = new EncodedParameters("abc");
            var list = new EncodedList(new string[] { "foo", "foo" }, "hash");
            subject.RequestHeaders = list;

            var values = subject.Encode();

            values.Should().ContainKey("h");
            values["h"].Should().BeAssignableTo<object[]>();
            var parts = (object[])values["h"];
            var keys = (IEnumerable<string>)parts[0];
            keys.Count().Should().Be(2);
            keys.Should().ContainInOrder(new string[] { "foo", "foo" });
            var value = (string)parts[1];
            value.Should().Be("hash");
        }

        [Fact]
        public void encoding_should_emit_body()
        {
            var subject = new EncodedParameters("abc");
            subject.BodyHash = "hash";

            var values = subject.Encode();

            values.Should().ContainKey("b");
            var body = (string)values["b"];
            body.Should().Be("hash");
        }

        [Fact]
        public void from_json_should_require_param()
        {
            var values = new Dictionary<string, object>();

            Assert.Throws<ArgumentNullException>(() => EncodedParameters.FromJson((string)null));
            Assert.Throws<ArgumentException>(() => EncodedParameters.FromJson("not json"));
        }

        [Fact]
        public void decoding_should_require_access_token()
        {
            var values = new Dictionary<string, object>();
            Assert.Throws<ArgumentException>(() => new EncodedParameters(values));
        }

        [Fact]
        public void decoding_should_require_access_token_to_be_a_string()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", 5 }
            };
            Assert.Throws<ArgumentException>(() => new EncodedParameters(values));
        }

        [Fact]
        public void decoding_should_parse_access_token()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" }
            };
            var subject = new EncodedParameters(values);
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
            var subject = new EncodedParameters(values);
            subject.TimeStamp.Should().HaveValue();
            subject.TimeStamp.Value.Should().Be(5000);
        }

        [Fact]
        public void decoding_should_require_timestamp_to_be_a_number()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "ts", "5000" }
            };

            Assert.Throws<ArgumentException>(() => new EncodedParameters(values));
        }

        [Fact]
        public void decoding_should_parse_http_method()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "m", "PUT" }
            };
            var subject = new EncodedParameters(values);
            subject.HttpMethod.Should().Be("PUT");
        }

        [Fact]
        public void decoding_should_parse_host()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "u", "foo.com:123" }
            };
            var subject = new EncodedParameters(values);
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
            var subject = new EncodedParameters(values);
            subject.Path.Should().Be("/foo/bar");
        }

        [Fact]
        public void decoding_should_parse_query_params()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "q",  new object[] { new string[] { "a", "b", "c" }, "hash" } }
            };

            var subject = new EncodedParameters(values);
            subject.QueryParameters.Should().NotBeNull();
        }

        [Fact]
        public void decoding_should_parse_request_headers()
        {
            var values = new Dictionary<string, object>()
            {
                { "at", "token" },
                { "h",  new object[] { new string[] { "a", "b", "c" }, "hash" } }
            };
            var subject = new EncodedParameters(values);
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
            var subject = new EncodedParameters(values);
            subject.BodyHash.Should().Be("body");
        }

        [Fact]
        public void IsSame_should_fail_if_null_param()
        {
            var subject = new EncodedParameters("token");
            subject.IsSame(null).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_fail_if_token_differs()
        {
            var subject = new EncodedParameters("token1");
            var other = new EncodedParameters("token2");
            subject.IsSame(other).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_succeed_if_tokens_are_same()
        {
            var subject = new EncodedParameters("token");
            var other = new EncodedParameters("token");
            subject.IsSame(other).Should().BeTrue();
        }

        [Fact]
        public void IsSame_should_work_for_method()
        {
            var subject = new EncodedParameters("token");
            var other = new EncodedParameters("token");

            subject.HttpMethod = other.HttpMethod = "POST";
            subject.IsSame(other).Should().BeTrue();

            other.HttpMethod = "GET";
            subject.IsSame(other).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_work_for_host()
        {
            var subject = new EncodedParameters("token");
            var other = new EncodedParameters("token");

            subject.Host = other.Host = "foo.com";
            subject.IsSame(other).Should().BeTrue();

            other.Host = "bar.com";
            subject.IsSame(other).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_work_for_path()
        {
            var subject = new EncodedParameters("token");
            var other = new EncodedParameters("token");

            subject.Path = other.Path = "/path";
            subject.IsSame(other).Should().BeTrue();

            other.Path = "/not_path";
            subject.IsSame(other).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_work_for_body()
        {
            var subject = new EncodedParameters("token");
            var other = new EncodedParameters("token");

            subject.BodyHash = other.BodyHash = "hash";
            subject.IsSame(other).Should().BeTrue();

            other.BodyHash = "not_hash";
            subject.IsSame(other).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_work_for_query()
        {
            var subject = new EncodedParameters("token");
            var other = new EncodedParameters("token");

            var list = new EncodedList(new string[] { "a" }, "hash");

            subject.QueryParameters = other.QueryParameters = list;
            subject.IsSame(other).Should().BeTrue();

            other.QueryParameters = null;
            subject.IsSame(other).Should().BeFalse();

            other.QueryParameters = list;
            subject.QueryParameters = null;
            subject.IsSame(other).Should().BeFalse();

            other.QueryParameters = null;
            subject.QueryParameters = null;
            subject.IsSame(other).Should().BeTrue();
        }

        [Fact]
        public void IsSame_should_work_for_header()
        {
            var subject = new EncodedParameters("token");
            var other = new EncodedParameters("token");

            var list = new EncodedList(new string[] { "a" }, "hash");

            subject.RequestHeaders = other.RequestHeaders = list;
            subject.IsSame(other).Should().BeTrue();

            other.RequestHeaders = null;
            subject.IsSame(other).Should().BeFalse();

            other.RequestHeaders = list;
            subject.RequestHeaders = null;
            subject.IsSame(other).Should().BeFalse();

            other.RequestHeaders = null;
            subject.RequestHeaders = null;
            subject.IsSame(other).Should().BeTrue();
        }
    }
}
