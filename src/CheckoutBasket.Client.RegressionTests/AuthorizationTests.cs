using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace CheckoutBasket.Client.RegressionTests
{
    public class AuthorizationTests
    {
        private readonly TestSettings configuration;

        public AuthorizationTests()
        {
            this.configuration = new TestSettings();
        }

        [Fact]
        public async void ShouldGetTokenWithValidCredentials()
        {
            await GetTokenAsync(configuration.UserApiKey, configuration.UserApiSecret);
        }

        [Fact]
        public async void ShouldNotGetTokenWithInvalidCredentials()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await GetTokenAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            Assert.Equal("Cannot get security token: HTTP 401 Unauthorized", ex.Message);
        }        

        private async Task<string> GetTokenAsync(string username, string password){
            var postData = new Dictionary<string, string>() { { "username", username }, { "password", password } };
            string token;
            using (var httpClient = new HttpClient())
            {
                var getTokenResponse = await httpClient.PostAsync(configuration.ApiBaseUrl + "authorize/token", new FormUrlEncodedContent(postData));
                if (getTokenResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException($"Cannot get security token: HTTP {(int)getTokenResponse.StatusCode} {getTokenResponse.StatusCode}");
                }
                var json = await getTokenResponse.Content.ReadAsStringAsync();
                var tokenInfo = JsonConvert.DeserializeObject<AccessTokenDto>(json);
                token = tokenInfo.token;
            }

            Assert.NotNull(token);
            return token;
        }

        private class AccessTokenDto
        {
            public string token { get; set; }
            public int expires_int { get; set; }
        }
    }
}