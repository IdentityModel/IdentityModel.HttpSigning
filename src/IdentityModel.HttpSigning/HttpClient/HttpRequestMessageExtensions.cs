using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpSigning
{
    public static class HttpRequestMessageExtensions
    {
        public static async Task<byte[]> ReadBodyAsync(this HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (request.Content != null)
            {
                return await request.Content.ReadAsByteArrayAsync();
            }

            return null;
        }

        public static string GetAccessToken(this HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (HttpSigningConstants.AccessTokenParameterNames.AuthorizationHeaderScheme.Equals(request.Headers?.Authorization?.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return request.Headers.Authorization.Parameter;
            }

            return null;
        }

        public static void AddPopToken(this HttpRequestMessage request, string token)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (String.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            request.Headers.Authorization = new AuthenticationHeaderValue(HttpSigningConstants.AccessTokenParameterNames.AuthorizationHeaderScheme, token);
        }
    }
}
