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
    public class SigningOptionsTests
    {
        SigningOptions _subject = new SigningOptions();

        [Fact]
        public async Task no_access_token_should_not_create_parameters()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");

            var parameters = await _subject.CreateEncodingParametersAsync(request);
            parameters.Should().BeNull();
        }

        [Fact]
        public async Task no_settings_should_capture_just_access_token()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");

            var parameters = await _subject.CreateEncodingParametersAsync(request);
            parameters.AccessToken.Should().Be("token");
            parameters.HttpMethod.Should().BeNull();
            parameters.Host.Should().BeNull();
            parameters.UrlPath.Should().BeNull();
            parameters.QueryParameters.Should().BeNullOrEmpty();
            parameters.RequestHeaders.Should().BeNullOrEmpty();
            parameters.Body.Should().BeNull();
        }

        [Fact]
        public async Task http_method_should_be_captured()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");

            _subject.SignHttpMethod = true;
            var parameters = await _subject.CreateEncodingParametersAsync(request);

            parameters.HttpMethod.Should().Be(HttpMethod.Post);
        }

        [Fact]
        public async Task host_method_should_be_captured()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");

            _subject.SignHost = true;
            var parameters = await _subject.CreateEncodingParametersAsync(request);

            parameters.Host.Should().Be("foo.com");
        }

        [Fact]
        public async Task url_should_be_captured()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar?x=1");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");

            _subject.SignUrlPath = true;
            var parameters = await _subject.CreateEncodingParametersAsync(request);

            parameters.UrlPath.Should().Be("/bar");
        }

        [Fact]
        public async Task body_should_be_captured()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar?x=1");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");
            request.Content = new StringContent("hello");

            _subject.SignBody = true;
            var parameters = await _subject.CreateEncodingParametersAsync(request);

            Encoding.UTF8.GetString(parameters.Body).Should().Be("hello");
        }

        [Fact]
        public async Task all_query_params_should_be_captured()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar?x=1&y=2");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");
            request.Content = new StringContent("hello");

            _subject.SignAllQueryParameters = true;
            var parameters = await _subject.CreateEncodingParametersAsync(request);

            parameters.QueryParameters.Count.Should().Be(2);
            parameters.QueryParameters.Select(x => x.Key).Should().Contain(new string[] { "x", "y" });
            parameters.QueryParameters.Select(x => x.Value).Should().Contain(new string[] { "1", "2" });
        }

        [Fact]
        public async Task query_params_should_be_captured()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar?y=2&x=1&y=3");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");
            request.Content = new StringContent("hello");

            _subject.QueryParametersToSign = new string[] { "x", "y", "z" };
            var parameters = await _subject.CreateEncodingParametersAsync(request);

            parameters.QueryParameters.Count.Should().Be(3);
            parameters.QueryParameters.Select(x => x.Key).Should().ContainInOrder(new string[] { "x", "y", "y" });
            parameters.QueryParameters.Select(x => x.Value).Should().ContainInOrder(new string[] { "1", "2", "3" });
        }

        [Fact]
        public async Task request_headers_should_be_captured()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/bar?x=1&y=2");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");
            request.Headers.Add("c", "carrot");
            request.Headers.Add("a", "apple");
            request.Headers.Add("a", "pear");
            request.Content = new StringContent("hello");

            _subject.RequestHeadersToSign = new string[] { "a", "b", "c" };
            var parameters = await _subject.CreateEncodingParametersAsync(request);

            parameters.RequestHeaders.Count.Should().Be(3);
            parameters.RequestHeaders.Select(x => x.Key).Should().ContainInOrder(new string[] { "a", "a", "c" });
            parameters.RequestHeaders.Select(x => x.Value).Should().ContainInOrder(new string[] { "apple", "pear", "carrot" });
        }
    }
}
