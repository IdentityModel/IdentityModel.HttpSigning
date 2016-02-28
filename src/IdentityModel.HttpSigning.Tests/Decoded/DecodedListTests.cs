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
    public class DecodedListTests
    {
        [Fact]
        public void constructor_should_require_value()
        {
            Assert.Throws<ArgumentNullException>(() => new DecodedList(null));
        }

        [Fact]
        public void constructor_should_require_array()
        {
            Assert.Throws<ArgumentException>(() => new DecodedList("hello"));
        }

        [Fact]
        public void constructor_should_require_array_with_correct_size()
        {
            Assert.Throws<ArgumentException>(() => new DecodedList(new object[3]));
        }

        [Fact]
        public void constructor_should_require_array_with_string_collection_for_first_item()
        {
            Assert.Throws<ArgumentException>(() => new DecodedList(new object[2] { "wrong", "xoxo" }));
        }

        [Fact]
        public void constructor_should_require_string_for_second_item()
        {
            Assert.Throws<ArgumentException>(() => new DecodedList(new object[2] { new string[2], 5 }));
        }

        [Fact]
        public void constructor_should_expose_correct_values()
        {
            var values = new object[] 
            {
                new string[] { "a", "b", "c" },
                "hash"
            };
            var subject = new DecodedList(values);
            subject.Keys.Should().ContainInOrder(new string[] { "a", "b", "c" });
            subject.HashedValue.Should().Be("hash");
        }
    }
}
