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
        private readonly string _connectionString = "server=.;database=Smart_EcommerceV2;integrated security =true; trust server certificate = true ";

        public CartController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("{BuyerId}/{ItemId}/{Quantity}")]
        public async Task<IActionResult> DeleteItem(int BuyerId, int ItemId ,int Quantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_AddToCart", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", BuyerId);
                        command.Parameters.AddWithValue("@ItemID", ItemId);
                        command.Parameters.AddWithValue("@Quantity", Quantity);



                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Item deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        // DELETE: Delete an item
        [HttpDelete("{BuyerId}/{ItemId}")]
        public async Task<IActionResult> DeleteItem(int BuyerId, int ItemId )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_deleteFromCart", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", BuyerId);
                        command.Parameters.AddWithValue("@ItemID", ItemId);


                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Item deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetUserItems/{UserId}")]
        public async Task<IActionResult> GetUserItems(int UserId)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetFromCartById", connection))
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
