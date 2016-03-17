using IdentityModel.HttpSigning.Logging;
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
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly Signature _signature;
        private readonly RequestSigningOptions _options;

        public HttpSigningMessageHandler(Signature signature, RequestSigningOptions options, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            if (signature == null) throw new ArgumentNullException("signature");

            _signature = signature;
            _options = options ?? new RequestSigningOptions();
        }

        public HttpSigningMessageHandler(Signature signature, RequestSigningOptions options)
            : this(signature, options, new HttpClientHandler())
        {
        }

        public HttpSigningMessageHandler(Signature signature, HttpMessageHandler innerHandler)
            : this(signature, null, innerHandler)
        {
        }

        public HttpSigningMessageHandler(Signature signature)
            : this(signature, null, new HttpClientHandler())
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
                Logger.Debug("Encoding parameters recieved; signing and adding pop token");

                var token = _signature.Sign(parameters);
                request.AddPopToken(token);
            }
            else
            {
                Logger.Debug("No encoding parameters recieved; not adding pop token");
            }
        }
    }
}
