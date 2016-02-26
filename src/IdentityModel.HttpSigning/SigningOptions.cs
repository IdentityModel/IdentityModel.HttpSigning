using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.HttpSigning
{
    public class SigningOptions
    {
        public bool SignHttpMethod { get; set; }
        public bool SignHost { get; set; }
        public bool SignUrlPath { get; set; }
        public bool SignAllQueryParameters { get; set; }
        public IEnumerable<string> QueryParametersToSign { get; set; }
        public IEnumerable<string> RequestHeadersToSign { get; set; }
        public bool SignBody { get; set; }

        public async Task<EncodingParameters> CreateEncodingParametersAsync(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var token = request.GetAccessToken();
            if (token == null) return null;

            var parameters = new EncodingParameters(token);

            if (SignHttpMethod)
            {
                parameters.HttpMethod = request.Method;
            }

            if (SignHost)
            {
                parameters.Host = request.RequestUri.Host;
            }

            if (SignUrlPath)
            {
                parameters.UrlPath = request.RequestUri.AbsolutePath;
            }

            var query = request.RequestUri.ParseQueryString();

            var queryParamsToSign = QueryParametersToSign;
            if (SignAllQueryParameters)
            {
                queryParamsToSign = query.Keys.Cast<string>();
            }

            if (queryParamsToSign != null && queryParamsToSign.Any())
            {
                foreach (var key in queryParamsToSign)
                {
                    var value = query[key];
                    if (value != null)
                    {
                        parameters.QueryParameters.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
            }

            if (RequestHeadersToSign != null && RequestHeadersToSign.Any())
            {
                foreach (var key in RequestHeadersToSign)
                {
                    IEnumerable<string> values;
                    if (request.Headers.TryGetValues(key, out values))
                    {
                        foreach (var value in values)
                        {
                            parameters.RequestHeaders.Add(new KeyValuePair<string, string>(key, value));
                        }
                    }
                }
            }

            if (SignBody)
            {
                parameters.Body = await request.ReadBodyAsync();
            }

            return parameters;
        }
    }
}
