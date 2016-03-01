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
    public class EncodedListTests
    {
        [Fact]
        public void constructor_should_require_value()
        {
            Assert.Throws<ArgumentNullException>(() => new EncodedList(null));
            Assert.Throws<ArgumentNullException>(() => new EncodedList(null, "hash"));
        }

        [Fact]
        public void constructor_should_require_correct_array_structure()
        {
            Assert.Throws<ArgumentException>(() => new EncodedList("hello"));
            Assert.Throws<ArgumentException>(() => new EncodedList(5));
            Assert.Throws<ArgumentException>(() => new EncodedList(new object[1]));
            Assert.Throws<ArgumentException>(() => new EncodedList(new object[3]));
            Assert.Throws<ArgumentException>(() => new EncodedList(new object[] { new string[] { "a" }, "hash", 5 }));
            Assert.Throws<ArgumentException>(() => new EncodedList(new object[] { 5, "hash" }));
        }

        [Fact]
        public void encoding_should_emit_correct_array_length_and_item_types()
        {
            var keys = new string[] { "a", "b", "c" };
            var subject = new EncodedList(keys, "hash");

            var result = subject.Encode();
            result.Length.Should().Be(2);
            result[0].GetType().Should().BeAssignableTo<IEnumerable<string>>();
            ((IEnumerable<string>)result[0]).Should().Contain(new string[] { "a", "b", "c" });
            result[1].GetType().Should().Be<string>();
            ((string)result[1]).Should().Be("hash");
        }

        [Fact]
        public void decoding_should_expose_correct_values()
        {
            var values = new object[]
            {
                new string[] { "a", "b", "c" },
                "hash"
            };
            var subject = new EncodedList(values);
            subject.Keys.Should().ContainInOrder(new string[] { "a", "b", "c" });
            subject.HashedValue.Should().Be("hash");
        }

        [Fact]
        public void IsSame_should_fail_if_param_is_null()
        {
            var subject = new EncodedList(new string[] { "a", "b", "c" }, "hash");
            subject.IsSame(null).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_fail_if_arrays_diff_size()
        {
            var subject = new EncodedList(new string[] { "a", "b", "c" }, "hash");
            var other = new EncodedList(new string[] { "a", "b" }, "hash");
            subject.IsSame(other).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_fail_if_hash_values_differ()
        {
            var subject = new EncodedList(new string[] { "a", "b", "c" }, "hash1");
            var other = new EncodedList(new string[] { "a", "b", "c" }, "hash2");
            subject.IsSame(other).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_fail_if_array_items_do_not_match()
        {
            var subject = new EncodedList(new string[] { "a", "b", "c" }, "hash");
            var other = new EncodedList(new string[] { "a", "c", "b" }, "hash");
            subject.IsSame(other).Should().BeFalse();
        }

        [Fact]
        public void IsSame_should_succeed_if_values_match()
        {
            var subject = new EncodedList(new string[] { "a", "b", "c", "c", "c" }, "hash");
            var other = new EncodedList(new string[] { "a", "b", "c", "c", "c" }, "hash");
            subject.IsSame(other).Should().BeTrue();
        }
    }
}
