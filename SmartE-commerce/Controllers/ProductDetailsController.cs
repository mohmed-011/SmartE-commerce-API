using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Details")]
    [Authorize]
    public class ProductDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public ProductDetailsController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }


        [HttpPost("AddPhoneDetails")]
        public async Task<IActionResult> AddPhoneDetails(PhonePost phone,string Item_ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_AddPhonesv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Item_ID", Item_ID);
                        command.Parameters.AddWithValue("@RAM", phone.RAM);
                        command.Parameters.AddWithValue("@Memory", phone.Memory);
                        command.Parameters.AddWithValue("@CPU", phone.CPU);
                        command.Parameters.AddWithValue("@Color", phone.Color);
                        command.Parameters.AddWithValue("@Screen_Size", phone.Screen_Size);



                        await command.ExecuteNonQueryAsync();
                    }
                }
                var response = new Dictionary<string, object>();
                response["messageToUser"] = $"Item {Item_ID} Details Added successfully.";
                response["message"] = "success";
                return Ok(response);

            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
