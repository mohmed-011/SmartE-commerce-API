using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Wishlist")]
    [Authorize]


    public class WishlistController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public WishlistController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddItemToWishlist(int BuyerId, int ItemId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_AddToWishListv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", BuyerId);
                        command.Parameters.AddWithValue("@ItemID", ItemId);
                        



                        await command.ExecuteNonQueryAsync();
                    }
                }
                var response = new Dictionary<string, object>();
                response["messageToUser"] = $"Item {ItemId} Added successfully to Wishlist.";

                response["message"] = "success";

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("RemoveProduct")]
        public async Task<IActionResult> DeleteItem(int BuyerId, int ItemId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_deleteFromWishListv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BuyerID", BuyerId);
                        command.Parameters.AddWithValue("@ItemID", ItemId);


                        await command.ExecuteNonQueryAsync();
                    }
                }
                var response = new Dictionary<string, object>();
                response["messageToUser"] = $"Item {ItemId} Deletet Successfully From Wishlist.";

                response["message"] = "success";

                return Ok(response);
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetUserProducts")]
        public async Task<IActionResult> GetUserItems(int UserId)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetFromWishListByIdv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pass parameters to the stored procedure
                        command.Parameters.AddWithValue("@BuyerID", UserId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var Mas = new Dictionary<string, object>();

                            Mas["message"] = "success";
                            resultList.Add(Mas);

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
