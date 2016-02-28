// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace IdentityModel.HttpSigning
{
    public class EncodingHeaderList : EncodingList
    {
        public EncodingHeaderList(ICollection<KeyValuePair<string, string>> list)
            : base(list, HttpSigningConstants.HashedRequestHeaderSeparators.KeyValueSeparator, HttpSigningConstants.HashedRequestHeaderSeparators.ParameterSeparator, true)
        {
        }
    }
}
