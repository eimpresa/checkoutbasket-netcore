using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutBasket.RegressionTests
{
    public class CheckoutBasketTestClient
    {
        private readonly string baseUrl;
        private readonly NetworkCredential credentials;
        private readonly HttpClient client;

        public CheckoutBasketTestClient(string baseUrl, NetworkCredential credentials)
        {
            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            this.client = new HttpClient();
        }

        public async Task<ServiceResponse<Guid>> CreateProduct(string name, uint quantity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { name, quantity }));
            content.Headers.ContentType.MediaType = "application/json";
            var res = await PostAsync("products", content);
            switch (res.StatusCode)
            {
                case HttpStatusCode.Created:
                    var productId = Guid.Parse(res.Headers.Location.OriginalString.Split("/").Last());
                    return ServiceResponse.Ok<Guid>(productId);
                case HttpStatusCode.BadRequest:
                    var json = await res.Content.ReadAsStringAsync();
                    return ServiceResponse.Fail<Guid>(JsonConvert.DeserializeObject<string[]>(json));
                default:
                    throw new Exception($"Cannot create product: unexpected HTTP status code (expected: {HttpStatusCode.Created}; actual: {res.StatusCode})");
            }
        }

        public async Task<ProductInfoContract> GetProduct(Guid productId)
        {
            var res = await GetAsync($"products/{productId}");
            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:
                    var jsonString = await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ProductInfoContract>(jsonString);
                case HttpStatusCode.NotFound:
                    throw new KeyNotFoundException($"Product not found (product id: {productId})");
                default:
                    throw new Exception($"Cannot get product: unexpected HTTP status code (expected: {HttpStatusCode.OK}; actual: {res.StatusCode})");
            }
        }

        private async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await AuthenticatedRequest(async client =>
            {
                return await client.DeleteAsync(this.baseUrl + url);
            });

        }

        private async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await AuthenticatedRequest(async client =>
            {
                return await client.GetAsync(this.baseUrl + url);
            });

        }

        private async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return await AuthenticatedRequest(async client =>
            {
                return await client.PostAsync(this.baseUrl + url, content);
            });

        }

        private async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return await AuthenticatedRequest(async client =>
            {
                return await client.PutAsync(this.baseUrl + url, content);
            });

        }

        private async Task<HttpResponseMessage> AuthenticatedRequest(Func<HttpClient, Task<HttpResponseMessage>> request)
        {
            var postData = new Dictionary<string, string>() { { "username", credentials.UserName }, { "password", credentials.Password } };
            string token;
            using (var client = new HttpClient())
            {
                var getTokenResponse = await client.PostAsync(baseUrl + "authorize/token", new FormUrlEncodedContent(postData));
                if (getTokenResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("Cannot get security token: HTTP " + getTokenResponse.StatusCode);
                }
                var json = await getTokenResponse.Content.ReadAsStringAsync();
                var tokenInfo = JsonConvert.DeserializeObject<AuthorizationToken>(json);
                token = tokenInfo.token;
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                return await request(client);
            }
        }

        private class AuthorizationToken
        {
            public string token { get; set; }
        }
    }
}
