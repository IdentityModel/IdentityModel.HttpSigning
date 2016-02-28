using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Net.Http;
using IdentityModel.HttpSigning;
using System.IO;

namespace IdentityModel.HttpSigning.Tests
{
    public class HttpRequestMessageExtensionsTests
    {
        [Fact]
        public async Task when_no_content_ReadBodyAsync_should_return_null()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");

            var bytes = await request.ReadBodyAsync();
            bytes.Should().BeNull();
        }

        [Fact]
        public async Task for_byte_array_content_ReadBodyAsync_should_return_bytes()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            var content = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 };
            request.Content = new ByteArrayContent(content);

            var bytes = await request.ReadBodyAsync();
            bytes.Should().ContainInOrder(content);
        }

        [Fact]
        public async Task for_form_urlencoded_content_ReadBodyAsync_should_return_bytes()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            var content = new Dictionary<string, string>
            {
                {"a", "apple"},
                {"b", "banana"},
            };
            request.Content = new FormUrlEncodedContent(content);

            var bytes = await request.ReadBodyAsync();
            var body = Encoding.UTF8.GetString(bytes);
            body.Should().Be("a=apple&b=banana");
        }

        [Fact]
        public async Task for_string_content_ReadBodyAsync_should_return_bytes()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            request.Content = new StringContent("hello");

            var bytes = await request.ReadBodyAsync();
            var body = Encoding.UTF8.GetString(bytes);
            body.Should().Be("hello");
        }

        [Fact]
        public async Task for_stream_content_ReadBodyAsync_should_return_bytes()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            var content = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 };
            var ms = new MemoryStream(content);
            request.Content = new StreamContent(ms);

            var bytes = await request.ReadBodyAsync();
            bytes.Should().ContainInOrder(content);
        }

        [Fact]
        public void no_authorization_header_GetAccessToken_should_return_null()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");

            var token = request.GetAccessToken();
            token.Should().BeNull();
        }

        [Fact]
        public void authorization_header_not_pop_GetAccessToken_should_return_null()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("foo", "bar");
            var token = request.GetAccessToken();
            token.Should().BeNull();
        }

        [Fact]
        public void authorization_header_with_pop_GetAccessToken_should_return_token()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("PoP", "token");
            var token = request.GetAccessToken();
            token.Should().Be("token");
        }

        [Fact]
        public void AddPopToken_should_set_authorization_header()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            request.AddPopToken("token");

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.Scheme.Should().Be("PoP");
            request.Headers.Authorization.Parameter.Should().Be("token");
        }
    }
}
