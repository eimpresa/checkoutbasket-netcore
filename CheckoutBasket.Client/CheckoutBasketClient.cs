using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutBasket.Client
{
    public class CheckoutBasketClient
    {
        private readonly string baseUrl;
        private readonly NetworkCredential credentials;
        private readonly HttpClient client;

        public CheckoutBasketClient(string baseUrl, NetworkCredential credentials)
        {
            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            this.client = new HttpClient();
        }

        public async Task<ServiceResponse<OrderCreated>> CreateOrder()
        {
            var res = await PostAsync("orders", new StringContent(""));
            switch (res.StatusCode)
            {
                case HttpStatusCode.Created:
                    var orderId = Guid.Parse(res.Headers.Location.OriginalString.Split("/").Last());
                    return new ServiceResponse<OrderCreated>(new OrderCreated { Id = orderId });
                case HttpStatusCode.BadRequest:
                    var json = await res.Content.ReadAsStringAsync();
                    return new ServiceResponse<OrderCreated>(false, JsonConvert.DeserializeObject<IEnumerable<string>>(json));
                default:
                    throw new Exception($"Cannot create order: unexpected HTTP status code (expected: {HttpStatusCode.Created}; actual: {res.StatusCode})");
            }
        }

        public async Task<ServiceResponse> AddItem(Guid orderId, Guid productId, uint quantity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { productId, quantity }));
            content.Headers.ContentType.MediaType = "application/json";
            var res = await PutAsync($"orders/{orderId}", content);
            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new ServiceResponse(true, null);
                case HttpStatusCode.BadRequest:
                    var json = await res.Content.ReadAsStringAsync();
                    return new ServiceResponse(false, JsonConvert.DeserializeObject<IEnumerable<string>>(json));
                default:
                    throw new Exception($"Cannot add item to order: unexpected HTTP status code (expected: {HttpStatusCode.OK}; actual: {res.StatusCode})");
            }
        }

        public async Task<ServiceResponse> DeleteItem(Guid orderId, Guid productId)
        {
            var res = await DeleteAsync($"orders/{orderId}/{productId}");
            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new ServiceResponse(true, null);
                case HttpStatusCode.BadRequest:
                    var json = await res.Content.ReadAsStringAsync();
                    return new ServiceResponse(false, JsonConvert.DeserializeObject<IEnumerable<string>>(json));
                default:
                    throw new Exception($"Cannot delete item: unexpected HTTP status code (expected: {HttpStatusCode.Created}; actual: {res.StatusCode})");
            }
        }

        public async Task ClearOrder(Guid orderId)
        {
            var orderInfo = await GetOrder(orderId);
            foreach (var item in orderInfo.Items)
            {
                await DeleteItem(orderId, item.ProductId);
            }
        }

        public async Task<OrderInfo> GetOrder(Guid orderId)
        {
            var res = await GetAsync($"orders/{orderId}");
            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:
                    var jsonString = await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<OrderInfo>(jsonString);
                case HttpStatusCode.NotFound:
                    throw new KeyNotFoundException($"Order not found (order id: {orderId})");
                default:
                    throw new Exception($"Cannot create order: unexpected HTTP status code (expected: {HttpStatusCode.OK}; actual: {res.StatusCode})");
            }
        }

        public async Task<ServiceResponse> UpdateItem(Guid orderId, Guid productId, uint quantity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { quantity }));
            content.Headers.ContentType.MediaType = "application/json";
            var res = await PutAsync($"orders/{orderId}/{productId}", content);
            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new ServiceResponse(true, null);
                case HttpStatusCode.BadRequest:
                    var json = await res.Content.ReadAsStringAsync();
                    return new ServiceResponse(false, JsonConvert.DeserializeObject<IEnumerable<string>>(json));
                default:
                    throw new Exception($"Cannot add item to order: unexpected HTTP status code (expected: {HttpStatusCode.OK}; actual: {res.StatusCode})");
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