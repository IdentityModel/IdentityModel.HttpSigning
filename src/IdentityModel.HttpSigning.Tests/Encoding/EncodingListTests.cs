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
    public class EncodingListTests
    {
        [Fact]
        public void constructor_should_require_items()
        {
            Assert.Throws<ArgumentNullException>(() => new EncodingList(null, "-", ",", false));
            Assert.Throws<ArgumentException>(() => new EncodingList(new List<KeyValuePair<string, string>>(), "-", ",", false));
        }

        [Theory]
        [InlineData(new string[] { "a" }, new string[] { "apple" })]
        [InlineData(new string[] { "a", "b" }, new string[] { "apple", "banana" })]
        [InlineData(new string[] { "a", "b" }, new string[] { "apple", "banana" })]
        [InlineData(new string[] { "a", "b", "c" }, new string[] { "apple", "banana", "carrot" })]
        [InlineData(new string[] { "a", "b", "b", "b" }, new string[] { "apple", "banana", "carrot", "duck" })]
        public void constructor_should_capture_correct_keys(string[] keys, string[] values)
        {
            var items = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < keys.Length; i++)
            {
                items.Add(new KeyValuePair<string, string>(keys[i], values[i]));
            }
            var subject = new EncodingList(items, "-", ",", false);

            subject.Keys.Count().Should().Be(keys.Length);
            subject.Keys.Should().ContainInOrder(keys);
        }

        [Theory]
        [InlineData(new string[] { "a" }, new string[] { "apple" }, "a-apple")]
        [InlineData(new string[] { "A" }, new string[] { "apple" }, "A-apple")]
        [InlineData(new string[] { "a", "b" }, new string[] { "apple", "banana" }, "a-apple,b-banana")]
        [InlineData(new string[] { "a", "B" }, new string[] { "apple", "banana" }, "a-apple,B-banana")]
        [InlineData(new string[] { "A", "B" }, new string[] { "apple", "banana" }, "A-apple,B-banana")]
        [InlineData(new string[] { "a", "b", "c" }, new string[] { "apple", "banana", "carrot" }, "a-apple,b-banana,c-carrot")]
        [InlineData(new string[] { "a", "b", "b", "b" }, new string[] { "apple", "banana", "carrot", "duck" }, "a-apple,b-banana,b-carrot,b-duck")]
        public void list_constructor_should_capture_values_correctly(string[] keys, string[] values, string expected)
        {
            var items = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < keys.Length; i++)
            {
                items.Add(new KeyValuePair<string, string>(keys[i], values[i]));
            }
            var subject = new EncodingList(items, "-", ",", false);

            subject.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData(new string[] { "a" }, new string[] { "apple" }, "a-apple")]
        [InlineData(new string[] { "A" }, new string[] { "apple" }, "a-apple")]
        [InlineData(new string[] { "a", "B" }, new string[] { "apple", "banana" }, "a-apple,b-banana")]
        [InlineData(new string[] { "A", "B" }, new string[] { "apple", "banana" }, "a-apple,b-banana")]
        public void lower_casing_keys_should_capture_values_correctly(string[] keys, string[] values, string expected)
        {
            var items = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < keys.Length; i++)
            {
                items.Add(new KeyValuePair<string, string>(keys[i], values[i]));
            }
            var subject = new EncodingList(items, "-", ",", true);

            subject.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData(new string[] { "a" }, new string[] { "apple" })]
        [InlineData(new string[] { "a", "b" }, new string[] { "apple", "banana" })]
        [InlineData(new string[] { "a", "b", "c" }, new string[] { "apple", "banana", "carrot" })]
        [InlineData(new string[] { "a", "b", "b", "b" }, new string[] { "apple", "banana", "carrot", "duck" })]
        public void ToEncodedArray_should_return_correct_keys(string[] keys, string[] values)
        {
            var items = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < keys.Length; i++)
            {
                items.Add(new KeyValuePair<string, string>(keys[i], values[i]));
            }
            var subject = new EncodingList(items, "-", ",", false);
            var result = subject.Encode();
            result.Keys.Count().Should().Be(keys.Length);
            result.Keys.Should().ContainInOrder(keys);
        }

        [Theory]
        [InlineData(new string[] { "a" }, new string[] { "apple" }, "kfe15NCuoBs6V3HQSB4sgd1kNwL_NqvOla2HjXA9RaU")]
        [InlineData(new string[] { "a", "b" }, new string[] { "apple", "banana" }, "6zg6ro9gww9sYg8Hea3eRGl15uptcDNTH30v7Qyz9HU")]
        [InlineData(new string[] { "a", "b", "c" }, new string[] { "apple", "banana", "carrot" }, "6QUuhMw9asN1rSD7eXudXE2IXIC8_IpF-V2OEPH5APs")]
        [InlineData(new string[] { "a", "b", "b", "b" }, new string[] { "apple", "banana", "carrot", "duck" }, "LS_08JNFaQcuESAa9oP4AZA-DWovXfcUGOYTCjNHU7c")]
        public void ToEncodedArray_should_encode_values_correctly(string[] keys, string[] values, string expected)
        {
            var items = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < keys.Length; i++)
            {
                items.Add(new KeyValuePair<string, string>(keys[i], values[i]));
            }
            var subject = new EncodingList(items, "-", ",", false);
            var result = subject.Encode();
            result.HashedValue.Should().Be(expected);
        }
    }
}
