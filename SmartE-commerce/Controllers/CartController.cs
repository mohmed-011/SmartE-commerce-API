using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Cart")]
    //[Authorize]

    public class CartController : ControllerBase
    {

        private readonly ApplicationDbContext _dbContext;
        //private readonly string _connectionString = $"server=.;database=Smart_EcommerceV4;integrated security =true; trust server certificate = true ";
        private readonly string _connectionString;

        public CartController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }



        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddItem(int BuyerId, string ItemId, int Quantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_AddToCartv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", BuyerId);
                        command.Parameters.AddWithValue("@ItemID", ItemId);
                        command.Parameters.AddWithValue("@Quantity", Quantity);



                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok($"Item {ItemId} Added successfully to Cart.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("UpdateToCart")]
        public async Task<IActionResult> UpdateItem(int BuyerId, string ItemId, int Quantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_UpdateFromCartv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", BuyerId);
                        command.Parameters.AddWithValue("@ItemID", ItemId);
                        command.Parameters.AddWithValue("@Quantity", Quantity);



                        await command.ExecuteNonQueryAsync();
                    }
                }

                var Result = await GetUserItems(BuyerId);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("RemoveFromCart")]
        public async Task<IActionResult> DeleteItem(int BuyerId, string ItemId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_deleteFromCartv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", BuyerId);
                        command.Parameters.AddWithValue("@ItemID", ItemId);


                        await command.ExecuteNonQueryAsync();
                    }
                }

                var Result = await GetUserItems(BuyerId);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("EmptyCart")]
        public async Task<IActionResult> EmptyCart(int BuyerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_EmptyBuyerCartv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", BuyerId);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                var Result = await GetUserItems(BuyerId);
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetUserCart")]
        public async Task<IActionResult> GetUserItems(int UserId)
        {
            var resultList = new List<Dictionary<string, object>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetFromCartByIdv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", UserId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }
                                resultList.Add(row);
                            }
                        }
                    }
                }
                return Ok(resultList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetUserCartPayment")]
        public async Task<IActionResult> GetUserItemsPayment(int UserId)
        {
            var resultList = new List<Dictionary<string, object>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetFromCartByForPaymentIdv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", UserId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }
                                resultList.Add(row);
                            }
                        }
                    }
                }
                return Ok(resultList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SmartE_commerce.Services;

//namespace SmartE_commerce.Controllers
//{
//    [ApiController]
//    [Route("Cart")]
//    [Authorize]
//    public class CartController : ControllerBase
//    {
//        private readonly CartService _cartService;

//        public CartController(CartService cartService)
//        {
//            _cartService = cartService;
//        }

//        [HttpPost("AddToCart")]
//        public async Task<IActionResult> AddItem(int BuyerId, string ItemId, int Quantity)
//        {
//            try
//            {
//                await _cartService.AddItemAsync(BuyerId, ItemId, Quantity);
//                return Ok($"Item {ItemId} added successfully to Cart.");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        [HttpPut("UpdateToCart")]
//        public async Task<IActionResult> UpdateItem(int BuyerId, string ItemId, int Quantity)
//        {
//            try
//            {
//                await _cartService.UpdateItemAsync(BuyerId, ItemId, Quantity);
//                var result = await _cartService.GetUserItemsAsync(BuyerId);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        [HttpDelete("RemoveFromCart")]
//        public async Task<IActionResult> DeleteItem(int BuyerId, string ItemId)
//        {
//            try
//            {
//                await _cartService.DeleteItemAsync(BuyerId, ItemId);
//                var result = await _cartService.GetUserItemsAsync(BuyerId);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        [HttpDelete("EmptyCart")]
//        public async Task<IActionResult> EmptyCart(int BuyerId)
//        {
//            try
//            {
//                await _cartService.EmptyCartAsync(BuyerId);
//                var result = await _cartService.GetUserItemsAsync(BuyerId);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        [HttpGet("GetUserCart")]
//        public async Task<IActionResult> GetUserItems(int UserId)
//        {
//            try
//            {
//                var result = await _cartService.GetUserItemsAsync(UserId);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }
//    }
//}
