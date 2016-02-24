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
    public class EncodedHeaderListTests
    {
        [Theory]
        [InlineData(new string[] { "a" }, new string[] { "apple" }, "a: apple")]
        [InlineData(new string[] { "a", "b" }, new string[] { "apple", "banana" }, "a: apple\nb: banana")]
        [InlineData(new string[] { "a", "b", "c" }, new string[] { "apple", "banana", "carrot" }, "a: apple\nb: banana\nc: carrot")]
        [InlineData(new string[] { "a", "b", "b", "b" }, new string[] { "apple", "banana", "carrot", "duck" }, "a: apple\nb: banana\nb: carrot\nb: duck")]
        public void constructor_should_capture_values_correctly(string[] keys, string[] values, string expected)
        {
            var items = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < keys.Length; i++)
            {
                items.Add(new KeyValuePair<string, string>(keys[i], values[i]));
            }
            var subject = new EncodedHeaderList(items);

            subject.Value.Should().Be(expected);
        }

        [Fact(Skip ="RFC has a typo")]
        public void encoding_should_match_example_in_RFC()
        {
            // https://tools.ietf.org/html/draft-ietf-oauth-signed-http-request-02#section-3.2
            // Content-Type: application/json, Etag: 742-3u8f34-3r2nvv3
            // "h": [["content-type", "etag"], "bZA981YJBrPlIzOvplbu3e7ueREXXr38vSkxIBYOaxI"]
            // should be: P6z5XN4tTzHkfwe3XO1YvVUIurSuhvh_UG10N_j-aGs

            var items = new List<KeyValuePair<string, string>>();
            items.Add(new KeyValuePair<string, string>("Content-Type", "application/json"));
            items.Add(new KeyValuePair<string, string>("Etag", "742-3u8f34-3r2nvv3"));
            var subject = new EncodedHeaderList(items);

            subject.Keys.Count().Should().Be(2);
            subject.Keys.Should().ContainInOrder(new string[] { "content-type", "etag" });
            subject.Value.Should().Be("content-type: application/json" + '\n' + "etag: 742-3u8f34-3r2nvv3");

            var result = subject.ToEncodedArray();
            var resultValue = (string)result[1];
            //resultValue.Should().Be("P6z5XN4tTzHkfwe3XO1YvVUIurSuhvh_UG10N_j-aGs");
            resultValue.Should().Be("bZA981YJBrPlIzOvplbu3e7ueREXXr38vSkxIBYOaxI");
        }
    }
}
