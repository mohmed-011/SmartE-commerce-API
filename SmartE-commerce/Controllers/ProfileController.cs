using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SmartE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public ProfileController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }

        [HttpGet]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = new Dictionary<string, object>(); // كائن رئيسي يحتوي على البيانات والصور
            var UserProfile = new Dictionary<string, object>(); // لتخزين بيانات المنتج
            var imagesData = new Dictionary<string, string>();
            var advNumbers = new Dictionary<string, string>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // جلب بيانات المنتج الأساسية
                    using (SqlCommand command = new SqlCommand("SP_GetUserProfile", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Buyer_ID", id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) // استخدام ReadAsync بدلاً من while لأننا نتوقع صفًا واحدًا فقط
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    UserProfile[reader.GetName(i)] = reader.GetValue(i);
                                }
                            }
                        }
                    }

                    // جلب صور المنتج  var x = productData["Brand_ID"];
                    using (SqlCommand command2 = new SqlCommand("SP_GetUserAddresses", connection))
                    {
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.AddWithValue("@Buyer_ID", id);

                        using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                        {
                           
                            while (await reader2.ReadAsync())
                            {
                                for (int i = 0; i < reader2.FieldCount; i++)
                                {
                                    imagesData[reader2.GetName(i)] = reader2.GetValue(i).ToString();

                                }
                            }
                        }
                    }

                    using (SqlCommand command2 = new SqlCommand("SP_GetUserNumbers", connection))
                    {
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.AddWithValue("@Buyer_ID", id);

                        using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                        {
                            int j = 1;
                            while (await reader2.ReadAsync())
                            {
                                for (int i = 0; i < reader2.FieldCount; i++)
                                {
                                    advNumbers[reader2.GetName(i)] = reader2.GetValue(i).ToString();

                                }
                                j++;
                            }
                        }
                    }



                    // إضافة البيانات والصور إلى كائن الاستجابة النهائي
                    response["UserProfile"] = UserProfile;
                    response["Adresses"] = imagesData;
                    response["advNumbers"] = advNumbers;




                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
