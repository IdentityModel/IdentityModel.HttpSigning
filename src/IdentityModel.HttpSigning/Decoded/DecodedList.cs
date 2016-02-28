// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;

namespace IdentityModel.HttpSigning
{
    public class DecodedList
    {
        public DecodedList(object list)
        {
            if (list == null) throw new ArgumentNullException("list");

            object[] arr = list as object[];
            if (arr == null) throw new ArgumentException("list is not an array");

            Decode(arr);
        }

        private void Decode(object[] arr)
        {
            if (arr.Length != 2) throw new ArgumentException("list does not have exactly two items");

            var keys = arr[0] as IEnumerable<string>;
            if (keys == null) throw new ArgumentException("first item in list is not array of strings");

            var value = arr[1] as string;
            if (value == null) throw new ArgumentException("second item in list is not a string");

            Keys = keys;
            HashedValue = value;
        }

        public IEnumerable<string> Keys { get; private set; }
        public string HashedValue { get; private set; }
    }
}
