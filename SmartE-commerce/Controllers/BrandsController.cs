using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Brand")]
    [Authorize]
    public class BrandsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString = $"server=.;database=Smart_EcommerceV4;integrated security =true; trust server certificate = true ";

        public BrandsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

      


        [HttpGet("GetAllBrands")]
        public async Task<IActionResult> GetAllBrands()
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetAllBrandsv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pass parameters to the stored procedure
                        

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



        [HttpGet("GetSpecificBrand")]
        public async Task<IActionResult> GetSpecificBrand(int id)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetSpecificBrandv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pass parameters to the stored procedure
                        command.Parameters.AddWithValue("@Brand_ID", id);


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
