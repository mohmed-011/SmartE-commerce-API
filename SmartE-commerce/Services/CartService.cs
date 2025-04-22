using SmartE_commerce.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class CartService
{
    private readonly HttpClient _httpClient;

    public CartService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CartResponse>> GetUserCartAsync(int userId)
    {
        try
        {
            string url = $"http://sm-ecommerce.runasp.net/Cart/GetUserCart?UserId={userId}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<List<CartResponse>>(json, options);

                return result ?? new List<CartResponse>();
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return new List<CartResponse>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return new List<CartResponse>();
        }
    }

}
