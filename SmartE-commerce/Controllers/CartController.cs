using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Cart")]
    public class CartController : ControllerBase
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString = $"server=.;database=Smart_EcommerceV4;integrated security =true; trust server certificate = true ";

        public CartController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddItem(int BuyerId, string ItemId ,int Quantity)
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

                return Ok($"Item {ItemId} Updated successfully to Cart.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        
        [HttpDelete("RemoveFromCart")]
        public async Task<IActionResult> DeleteItem(int BuyerId, string ItemId )
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

                return Ok($"Item {ItemId} deleted successfully.");
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

                return Ok("Cart Empty now.");
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

                        // Pass parameters to the stored procedure
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
