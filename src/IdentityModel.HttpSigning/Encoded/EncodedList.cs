// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityModel.HttpSigning
{
    public class EncodedList
    {
        public EncodedList(IEnumerable<string> keys, string hashValue)
        {
            if (keys == null || !keys.Any()) throw new ArgumentNullException("keys");
            if (String.IsNullOrWhiteSpace(hashValue)) throw new ArgumentNullException("hashValue");

            this.Keys = keys;
            this.HashedValue = hashValue;
        }

        public EncodedList(object list)
        {
            if (list == null) throw new ArgumentNullException("list");

            var collection = list as IEnumerable<object>;
            if (collection == null) throw new ArgumentException("list is not an array");

            object[] arr = collection.ToArray();
            Decode(arr);
        }

        public bool IsSame(EncodedList other)
        {
            if (other == null) return false;

            var arr1 = Keys.ToArray();
            var arr2 = other.Keys.ToArray();

            var len = arr1.Length;
            if (arr1.Length != arr2.Length)
            {
                return false;
            }

            if (HashedValue != other.HashedValue)
            {
                return false;
            }

            for (var i = 0; i < Keys.Count(); i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private void Decode(object[] arr)
        {
            if (arr.Length != 2) throw new ArgumentException("list does not have exactly two items");

            var items = arr[0] as IEnumerable<object>;
            if (items == null) throw new ArgumentException("first item in list is not array of strings");

            var keys = items.Select(x => Convert.ToString(x)).ToArray();
            if (keys.Length != items.Count()) throw new ArgumentException("first item in list is not array of strings");

            if (arr[1] == null) throw new ArgumentException("second item in list is not a string");

            var value = Convert.ToString(arr[1]);
            if (value == null) throw new ArgumentException("second item in list is not a string");

            Keys = keys;
            HashedValue = value;
        }

        public object[] Encode()
        {
            return new object[]
            {
                Keys, HashedValue
            };
        }

        public IEnumerable<string> Keys { get; private set; }
        public string HashedValue { get; private set; }
    }
}
