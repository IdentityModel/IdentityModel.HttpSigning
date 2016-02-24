// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpSigning
{
    public class EncodedQueryParameters : EncodedList
    {
        public EncodedQueryParameters(ICollection<KeyValuePair<string, string>> list)
            : base(list, HttpSigningConstants.HashedQuerySeparators.KeyValueSeparator, HttpSigningConstants.HashedQuerySeparators.ParameterSeparator, false)
        {
        }
    }
}
