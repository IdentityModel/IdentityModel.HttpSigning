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

namespace IdentityModel.HttpSigning.Tests
{
    public class EncodedQueryParametersTests
    {
        [Theory]
        [InlineData(new string[] { "a" }, new string[] { "apple" }, "a=apple")]
        [InlineData(new string[] { "a", "b" }, new string[] { "apple", "banana" }, "a=apple&b=banana")]
        [InlineData(new string[] { "a", "b", "c" }, new string[] { "apple", "banana", "carrot" }, "a=apple&b=banana&c=carrot")]
        [InlineData(new string[] { "a", "b", "b", "b" }, new string[] { "apple", "banana", "carrot", "duck" }, "a=apple&b=banana&b=carrot&b=duck")]
        public void constructor_should_capture_values_correctly(string[] keys, string[] values, string expected)
        {
            var items = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < keys.Length; i++)
            {
                items.Add(new KeyValuePair<string, string>(keys[i], values[i]));
            }
            var subject = new EncodedQueryParameters(items);

            subject.Value.Should().Be(expected);
        }

        [Fact]
        public void encoding_should_match_example_in_RFC()
        {
            // https://tools.ietf.org/html/draft-ietf-oauth-signed-http-request-02#section-3.1
            // "b=bar", "a=foo", "c=duck"
            // b=bar&a=foo&c=duck
            // "p": [["b", "a", "c"], "u4LgkGUWhP9MsKrEjA4dizIllDXluDku6ZqCeyuR-JY"]

            var items = new List<KeyValuePair<string, string>>();
            items.Add(new KeyValuePair<string, string>("b", "bar"));
            items.Add(new KeyValuePair<string, string>("a", "foo"));
            items.Add(new KeyValuePair<string, string>("c", "duck"));
            var subject = new EncodedQueryParameters(items);

            subject.Keys.Count().Should().Be(3);
            subject.Keys.Should().ContainInOrder(new string[] { "b", "a", "c" });
            subject.Value.Should().Be("b=bar&a=foo&c=duck");

            var result = subject.ToEncodedArray();
            var resultValue = (string)result[1];
            resultValue.Should().Be("u4LgkGUWhP9MsKrEjA4dizIllDXluDku6ZqCeyuR-JY");
        }
    }
}
