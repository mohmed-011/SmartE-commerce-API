using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Classes;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdressesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public AdressesController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");
        }



        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddItemToWishlist(BillingData billing, int UserId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SP_InsertShippingAddress", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FirstName", billing.FirstName);
                        command.Parameters.AddWithValue("@LastName", billing.LastName);
                        command.Parameters.AddWithValue("@Email", billing.Email);
                        command.Parameters.AddWithValue("@PhoneNumber", billing.PhoneNumber);
                        command.Parameters.AddWithValue("@Country", billing.Country);
                        command.Parameters.AddWithValue("@City", billing.City);
                        command.Parameters.AddWithValue("@PostalCode", billing.PostalCode);
                        command.Parameters.AddWithValue("@Street", billing.Street);
                        command.Parameters.AddWithValue("@Building", billing.Building);
                        command.Parameters.AddWithValue("@Floor", billing.Floor);
                        command.Parameters.AddWithValue("@Apartment", billing.Apartment);
                        command.Parameters.AddWithValue("@ShippingMethod", billing.ShippingMethod);
                        command.Parameters.AddWithValue("@State", billing.State);


                        command.Parameters.AddWithValue("@UserId", UserId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                var response = new Dictionary<string, object>();
                response["messageToUser"] = $"Address Added successfully to Wishlist.";

                response["message"] = "success";

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetUserAddresses")]
        public async Task<IActionResult> GetUserItems(int UserId)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SP_GetShippingAddressByUserId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pass parameters to the stored procedure
                        command.Parameters.AddWithValue("@UserId", UserId);

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
