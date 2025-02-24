using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public DashboardController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }

        [HttpGet("GetSellerProfits")]
        public async Task<IActionResult> GetSellerProfits(int Seller_id)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetSellerMonthlyMonyv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pass parameters to the stored procedure
                        command.Parameters.AddWithValue("@Seller_ID", Seller_id);


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
                var response = new Dictionary<string, object>();

                response["message"] = "success";
                response["Data"] = resultList;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
