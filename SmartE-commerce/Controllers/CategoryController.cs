using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Category")]
    [Authorize]

    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString ;

        public CategoryController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }

        [HttpGet]
        [Route("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetAllCategoryv4", connection))
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


        [HttpGet]
        [Route("GetAllSubCategory")]
        public async Task<IActionResult> GetAllSubCategory()
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetAllSubCategoryv4", connection))
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


        [HttpGet]
        [Route("GetSubCategoryByCategory")]
        public async Task<IActionResult> GetSubCategoryByCategory(int categoryId)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetSubCategorysv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pass parameters to the stored procedure
                        command.Parameters.AddWithValue("@CatID", categoryId);


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

        [HttpGet]
        [Route("GetSubCategoryDetails")]
        public async Task<IActionResult> GetSubCategoryDetails(int subCategoryId)
        {
            var resultList = new List<Dictionary<string, object>>();
            var BrandList = new List<Dictionary<string, object>>();
            var Finalresult = new Dictionary<string, object>();



            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetSubCategoryDetilsv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pass parameters to the stored procedure
                        command.Parameters.AddWithValue("@SubCatID", subCategoryId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i).ToString();
                                }

                                resultList.Add(row);
                            }
                        }
                    }
                }

                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetSubCategoryBrandsv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pass parameters to the stored procedure
                        command.Parameters.AddWithValue("@SubCatID", subCategoryId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i).ToString();
                                }

                                BrandList.Add(row);
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            Finalresult["Detils"] = resultList;
            Finalresult["Brands"] = BrandList;
            return Ok(Finalresult);


        }
    }
}
