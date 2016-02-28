using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Net.Http;

namespace IdentityModel.HttpSigning.Tests
{
    public class HttpSigningMessageHandlerTests
    {
        StubHttpMessageHandler _stubMessageHandler = new StubHttpMessageHandler();
        Signature _signature;
        HttpSigningMessageHandler _subject;

        public HttpSigningMessageHandlerTests()
        {
            _signature = new HS256Signature(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
            _subject = new HttpSigningMessageHandler(_signature, _stubMessageHandler);
        }

        [Fact]
        public async Task when_no_parameters_created_ProcessSignatureAsync_should_not_set_authorization_header()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            await _subject.ProcessSignatureAsync(request);

            request.Headers.Authorization.Should().BeNull();
        }

        [Fact]
        public async Task when_parameters_created_ProcessSignatureAsync_should_set_authorization_header()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("PoP", "token");
            await _subject.ProcessSignatureAsync(request);

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.Scheme.Should().Be("PoP");
        }
    }
}
