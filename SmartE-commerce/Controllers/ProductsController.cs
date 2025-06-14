using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartE_commerce.Data;
using SmartE_commerce.Dto;
using System.Data;
using System.Drawing;
using System.Reflection.PortableExecutable;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Products")]
    [Authorize]

    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public ProductsController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }


        [HttpGet]
        [Route("GetFilteredProducts")]
        public async Task<IActionResult> GetFilteredProducts(
    [FromQuery] decimal? minPrice,
    [FromQuery] int? categry,
    [FromQuery] int? subCategry,
    [FromQuery] int? sellerId,
    [FromQuery] decimal? maxPrice,
    [FromQuery] decimal? minRate,
    [FromQuery] decimal? maxRate,
    [FromQuery] bool? mostViewed,
    [FromQuery] bool? mostSold,
    [FromQuery] bool? newwest,
    [FromQuery] string? searchQuery,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _dbContext.Items.AsQueryable();

                query = query.Where(i => !i.SoftDelete);

                if (categry.HasValue)
                    query = query.Where(i => i.Category_ID == categry.Value);

                if (sellerId.HasValue)
                    query = query.Where(i => i.Seller_ID == sellerId.Value);

                if (subCategry.HasValue)
                    query = query.Where(i => i.Sub_Category_ID == subCategry.Value);

                if (minPrice.HasValue)
                    query = query.Where(i => i.Price_out >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where(i => i.Price_out <= maxPrice.Value);

                if (minRate.HasValue)
                    query = query.Where(i => i.Rate >= minRate.Value);

                if (maxRate.HasValue)
                    query = query.Where(i => i.Rate <= maxRate.Value);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                    query = query.Where(i => i.Item_Name.Contains(searchQuery) || i.Description.Contains(searchQuery));

                if (mostViewed.HasValue && mostViewed.Value && mostSold.HasValue && mostSold.Value)
                {
                    query = query.OrderByDescending(i => i.View_Count).ThenByDescending(i => i.Sold_Count);
                }
                else if (mostViewed.HasValue && mostViewed.Value)
                {
                    query = query.OrderByDescending(i => i.View_Count);
                }
                else if (mostSold.HasValue && mostSold.Value)
                {
                    query = query.OrderByDescending(i => i.Sold_Count);
                }
                else if (newwest.HasValue && newwest.Value)
                {
                    query = query.OrderByDescending(i => i.Crate_Date);
                }
                else
                {
                    query = query.OrderByDescending(i => i.Rate);
                }

                int totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                if (!items.Any())
                    return Ok(new { message = "No items found matching your filters." });

                var productDetails = await Task.WhenAll(items.Select(async item =>
                {
                    var productResult = await GetProductById(item.Item_ID);
                    return productResult is OkObjectResult okResult ? okResult.Value : null;
                }));

                var response = new
                {
                    message = "success",
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    Products = productDetails.Where(p => p != null)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving data.",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }



        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts([FromQuery] List<string> itemIds)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetAllProductsv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // 🟢 تحويل القائمة إلى String مفصولة بفواصل وتمريرها كـ `NVARCHAR`
                        string itemIdsString = string.Join(",", itemIds);
                        command.Parameters.AddWithValue("@ItemIDs", itemIdsString);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                var imagesData = new Dictionary<string, string>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);

                                    if (reader.GetName(i) == "Item_ID")
                                    {
                                        var itemId = reader.GetValue(i).ToString();

                                        using (SqlCommand command2 = new SqlCommand("Sp_GetItemImagesv4", connection))
                                        {
                                            command2.CommandType = CommandType.StoredProcedure;
                                            command2.Parameters.AddWithValue("@ItemID", itemId);

                                            using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                                            {
                                                int j = 1;
                                                while (await reader2.ReadAsync())
                                                {
                                                    imagesData[$"Item_Images-{j}"] = reader2.GetValue(0).ToString();
                                                    j++;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (imagesData.Count > 0)
                                    row["images"] = imagesData;

                                resultList.Add(row);
                            }
                        }
                    }
                }

                return Ok(new { Data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("GetProductById")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var response = new Dictionary<string, object>(); // كائن رئيسي يحتوي على البيانات والصور
            var productData = new Dictionary<string, object>(); // لتخزين بيانات المنتج
            var imagesData = new Dictionary<string, string>();
            var BrandData = new Dictionary<string, string>();
            var DitailsData = new Dictionary<string, string>(); // لتخزين الصور
            // لتخزين الصور
            // لتخزين الصور

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // جلب بيانات المنتج الأساسية
                    using (SqlCommand command = new SqlCommand("Sp_GetProductByIdv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ItemID", id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) // استخدام ReadAsync بدلاً من while لأننا نتوقع صفًا واحدًا فقط
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    productData[reader.GetName(i)] = reader.GetValue(i);
                                }
                            }
                        }
                    }

                    // جلب صور المنتج  var x = productData["Brand_ID"];
                    using (SqlCommand command2 = new SqlCommand("Sp_GetItemImagesv4", connection))
                    {
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.AddWithValue("@ItemID", id);

                        using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                        {
                            int j = 1;
                            while (await reader2.ReadAsync())
                            {
                                for (int i = 0; i < reader2.FieldCount; i++)
                                {
                                    imagesData[$"Item_Images-{j}"] = reader2.GetValue(i).ToString();

                                }
                                j++;
                            }
                        }
                    }

                    using (SqlCommand command2 = new SqlCommand("Sp_GetItemBrandv4", connection))
                    {
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.AddWithValue("@Brand_ID", productData["Brand_ID"]);

                        using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                        {
                            int j = 1;
                            while (await reader2.ReadAsync())
                            {
                                for (int i = 0; i < reader2.FieldCount; i++)
                                {
                                    BrandData[reader2.GetName(i)] = reader2.GetValue(i).ToString();

                                }
                                j++;
                            }
                        }
                    }

                    using (SqlCommand command2 = new SqlCommand("Sp_GetItemDetilas4", connection))
                    {
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.AddWithValue("@ItemID", id);

                        using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                        {

                            while (await reader2.ReadAsync())
                            {
                                for (int i = 0; i < reader2.FieldCount; i++)
                                {
                                    DitailsData[reader2.GetName(i)] = reader2.GetValue(i).ToString();

                                }

                            }
                        }
                    }
                    var ratingSummary = new Dictionary<string, object>();

                    using (SqlCommand command = new SqlCommand("GetProductRatingSummary", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Item_ID", id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                ratingSummary["AverageRating"] = reader["AverageRating"];
                                ratingSummary["TotalReviews"] = reader["TotalReviews"];
                                ratingSummary["FirstReviewDate"] = reader["FirstReviewDate"];
                                ratingSummary["LastReviewDate"] = reader["LastReviewDate"];
                            }
                        }
                    }

                    // إضافة البيانات والصور إلى كائن الاستجابة النهائي
                    response["Data"] = productData;
                    response["images"] = imagesData;
                    response["Brand"] = BrandData;
                    response["Detilas"] = DitailsData;
                    response["Rating"] = ratingSummary;



                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("DeleteProductById")]
        public async Task<IActionResult> EmptyCart(string ItemID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_DeleteItemv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ItemID", ItemID);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                var response = new
                {
                    message = "success",

                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateItem([FromBody] ItemUpdateDto updatedItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("UpdateItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Item_ID", updatedItem.Item_ID ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Image_Cover", updatedItem.Image_Cover ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Item_Name", updatedItem.Item_Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", updatedItem.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", updatedItem.Quantity);
                    command.Parameters.AddWithValue("@Price_in", updatedItem.Price_in);
                    command.Parameters.AddWithValue("@Price_out", updatedItem.Price_out);
                    command.Parameters.AddWithValue("@Discount", updatedItem.Discount);
                    command.Parameters.AddWithValue("@Category_ID", updatedItem.Category_ID);
                    command.Parameters.AddWithValue("@Sub_Category_ID", updatedItem.Sub_Category_ID);
                    command.Parameters.AddWithValue("@Brand_ID", updatedItem.Brand_ID);
                    command.Parameters.AddWithValue("@Crate_Date", updatedItem.Crate_Date);

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return Ok(
                            new
                            {
                                message = "success",
                                display = "Item updated successfully via stored procedure."
                            });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }


        [HttpPost("AddItemView")]
        public async Task<IActionResult> AddItemView(string Item_ID, int Buyer_ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Add_Item_View", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Item_ID", Item_ID);
                        command.Parameters.AddWithValue("@Buyer_ID", Buyer_ID);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                var response = new
                {
                    message = "success",
                    display = "Item view count updated successfully."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost("AddProductReview")]
        public async Task<IActionResult> AddProductReview(string Item_ID, int Buyer_ID, float Rating, string Comment)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("AddProductReview", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Item_ID", Item_ID);
                        command.Parameters.AddWithValue("@Buyer_ID", Buyer_ID);
                        command.Parameters.AddWithValue("@Rating", Rating);
                        command.Parameters.AddWithValue("@Comment", Comment);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                var response = new Dictionary<string, object>
                {
                    ["message"] = "success",
                    ["messageToUser"] = "Review added successfully."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetProductReviews")]
        public async Task<IActionResult> GetProductReviews(string Item_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("GetProductReviews", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Item_ID", Item_ID);

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

                var response = new
                {
                    message = "success",
                    data = resultList
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
