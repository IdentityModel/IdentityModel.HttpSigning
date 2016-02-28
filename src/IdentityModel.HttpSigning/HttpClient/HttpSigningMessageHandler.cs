using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.HttpSigning
{
    public class HttpSigningMessageHandler : DelegatingHandler
    {
        private readonly Signature _signature;
        private readonly RequestOptions _options;

        public HttpSigningMessageHandler(Signature signature, RequestOptions options, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            if (signature == null) throw new ArgumentNullException("signature");

            _signature = signature;
            _options = options ?? new RequestOptions();
        }

        public HttpSigningMessageHandler(Signature signature, HttpMessageHandler innerHandler)
            : this(signature, null, innerHandler)
        {
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await ProcessSignatureAsync(request);
            return await base.SendAsync(request, cancellationToken);
        }

        public async Task ProcessSignatureAsync(HttpRequestMessage request)
        {
            var parameters = await _options.CreateEncodingParametersAsync(request);
            if (parameters != null)
            {
                var token = _signature.Sign(parameters);
                request.AddPopToken(token);
            }
        }
    }
}
