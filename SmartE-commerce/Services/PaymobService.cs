using Newtonsoft.Json;
using SmartE_commerce.Classes;
using SmartE_commerce.Data;
using System.Net.Http.Headers;
using SmartE_commerce.Controllers;

namespace SmartE_commerce.Services
{
    public class PaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "ZXlKaGJHY2lPaUpJVXpVeE1pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR0Z6Y3lJNklrMWxjbU5vWVc1MElpd2ljSEp2Wm1sc1pWOXdheUk2TVRBek5UZzFNU3dpYm1GdFpTSTZJakUzTkRRek9UazFOakF1TXpZek5UZ3pJbjAuYzMxeFlVSXNzbFpTRU9LalQwbFlqUjZaN2wzcEpGYnBiZUpHcWtIaW1MQ1lGQzE3Zk9hMUUwYlNMbXBpTTBqRW9LdEJFaWg4VGk5LXMzZm1HUFliQ2c=";
        private readonly int _integrationId =5037623;
        private readonly int _iframeId = 911960;
        private readonly CartService _cartService;
        decimal totalAmount = 0;
        decimal TotalAmountWithCents = 0;
        public PaymobService(CartService cartService)
        {
            _httpClient = new HttpClient();
            this._cartService = cartService;
        }

        public async Task<string> GetAuthToken()
        {
            var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", new
            {
                api_key = _apiKey
            });



            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);
            return result.token;
        }

        public async Task<int> CreateOrder(string token , int userId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpClient = new HttpClient();
            var cartService = new CartService(httpClient);
            var cartItems = await cartService.GetUserCartAsync(userId);

            
            foreach (var item in cartItems)
            {
                totalAmount += (item.PriceOut * item.Quantity);
                TotalAmountWithCents += (item.PriceOut * item.Quantity) * 100;
            }
            var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", new
            {
                auth_token = token,
                delivery_needed = false,
                amount_cents = TotalAmountWithCents, // 100 جنيه
                currency = "EGP",
                items = new object[] {  }
               
            });
          
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            int orderId = result.id ?? throw new Exception("Paymob order creation failed: id is null");
            return orderId;
        }

        public async Task<string> GetPaymentKey(string token, int orderId , BillingData billingData , int integrationId , int userId, int addressID)
        {
            

            var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", new
            {
                auth_token = token,
                amount_cents = TotalAmountWithCents,
                expiration = 1000,
                order_id = orderId,
                billing_data =  new
                {
                    apartment = billingData.Apartment,
                    email = billingData.Email,
                    floor = billingData.Floor,
                    first_name = billingData.FirstName,
                    street = billingData.Street,
                    building = billingData.Building,
                    phone_number = billingData.PhoneNumber,
                    shipping_method = billingData.ShippingMethod,
                    postal_code = billingData.PostalCode,
                    city = billingData.City,
                    country = billingData.Country,
                    last_name = billingData.LastName,
                    state = billingData.State
                },
                currency = "EGP",
                integration_id = integrationId,
                redirect_url = $"https://sm-ecommerce.runasp.net/api/Payment/payment-redirect?userId={userId}&addressID={addressID}"
            });
            //https://localhost:7221/
            //https://sm-ecommerce.runasp.net/
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);
            return result.token;
        }

        public string GetPaymentUrl(string paymentKey)
        {
            return $"https://accept.paymob.com/api/acceptance/iframes/{_iframeId}?payment_token={paymentKey}";
        }

    }
}
