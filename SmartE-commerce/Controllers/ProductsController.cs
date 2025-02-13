using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Products")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString = "server=.;database=Smart_EcommerceV4;integrated security =true; trust server certificate = true ;MultipleActiveResultSets=True ";


        public ProductsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        [Route("GetFilteredProducts")]
        [Authorize]
        public async Task<IActionResult> GetFilteredProducts(
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] decimal? minRate,
    [FromQuery] decimal? maxRate,
    [FromQuery] bool? mostViewed,
    [FromQuery] bool? mostSold,
    [FromQuery] string? searchQuery)
        {
            try
            {
                var query = _dbContext.Items.AsQueryable();

                // 🔹 الفلترة حسب السعر
                if (minPrice.HasValue)
                    query = query.Where(i => i.Price_out >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where(i => i.Price_out <= maxPrice.Value);

                // 🔹 الفلترة حسب التقييم
                if (minRate.HasValue)
                    query = query.Where(i => i.Rate >= minRate.Value);

                if (maxRate.HasValue)
                    query = query.Where(i => i.Rate <= maxRate.Value);

                // 🔹 البحث بالاسم أو الوصف
                if (!string.IsNullOrWhiteSpace(searchQuery))
                    query = query.Where(i => i.Item_Name.Contains(searchQuery) || i.Description.Contains(searchQuery));

                // 🔹 ترتيب النتائج
                if (mostViewed.HasValue && mostViewed.Value && mostSold.HasValue && mostSold.Value)
                {
                    query = query.OrderByDescending(i => i.View_Count)
                                 .ThenByDescending(i => i.Sold_Count);
                }
                else if (mostViewed.HasValue && mostViewed.Value)
                {
                    query = query.OrderByDescending(i => i.View_Count);
                }
                else if (mostSold.HasValue && mostSold.Value)
                {
                    query = query.OrderByDescending(i => i.Sold_Count);
                }
                else
                {
                    // 🔹 إذا لم يتم اختيار ترتيب معين، ترتيب افتراضي حسب التقييم
                    query = query.OrderByDescending(i => i.Rate);
                }

                // 🔹 جلب قائمة الـ Item_IDs
                var itemIds = await query.Select(i => i.Item_ID).ToListAsync();

                if (!itemIds.Any())
                    return Ok(new { message = "No items found matching your filters." });

                // 🔹 جلب تفاصيل كل منتج باستخدام `GetProductById`
                var productDetails = new List<object>();

                foreach (var itemId in itemIds)
                {
                    var productResult = await GetProductById(itemId);
                    if (productResult is OkObjectResult okResult)
                    {
                        productDetails.Add(okResult.Value);
                    }
                }

                return Ok(productDetails);
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


        //[HttpGet]
        //[Route("GetAllProducts")]
        //public async Task<IActionResult> GetAllProducts()
        //{
        //    var resultList = new List<Dictionary<string, object>>(); // List to store each product's data

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            using (SqlCommand command = new SqlCommand("Sp_GetAllProductsv4", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;


        //                using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var row = new Dictionary<string, object>();
        //                        var imagesData = new Dictionary<string, string>(); // Dictionary to store images for each product

        //                        for (int i = 0; i < reader.FieldCount; i++)
        //                        {
        //                            row[reader.GetName(i)] = reader.GetValue(i);
        //                            if (reader.GetName(i) == "Item_ID") // Once Item_ID is found
        //                            {
        //                                var itemId = reader.GetValue(i).ToString();

        //                                // Now fetch the images for this Item_ID
        //                                using (SqlCommand command2 = new SqlCommand("Sp_GetItemImagesv4", connection))
        //                                {
        //                                    command2.CommandType = CommandType.StoredProcedure;
        //                                    command2.Parameters.AddWithValue("@ItemID", itemId);

        //                                    using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
        //                                    {
        //                                        int j = 1;
        //                                        while (await reader2.ReadAsync())
        //                                        {
        //                                            // Add images to the dictionary with a dynamic key
        //                                            imagesData[$"Item_Images-{j}"] = reader2.GetValue(0).ToString();
        //                                            j++;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        // Add imagesData to the current product row
        //                        if (imagesData.Count > 0)
        //                        {
        //                            row["images"] = imagesData;
        //                        }

        //                        // Add the product row to the result list
        //                        resultList.Add(row);
        //                    }
        //                }
        //            }
        //        }

        //        return Ok(new { Data = resultList });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}



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
            var imagesData = new Dictionary<string, string>(); // لتخزين الصور

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

                    // جلب صور المنتج
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
                }

                // إضافة البيانات والصور إلى كائن الاستجابة النهائي
                response["Data"] = productData;
                response["images"] = imagesData;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }












        [HttpPost]
        [Route("InsertProduct")]
        public  ActionResult<int> CreateProduct(Product product)
        {
            product.Item_ID = 0;
            _dbContext.Set<Product>().Add(product);
             _dbContext.SaveChanges();
            return Ok(product.Item_ID);
        }

        [HttpPut]
        [Route("UpdateProduct")]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] Item updatedItem)
        {
            if (id != updatedItem.Item_ID)
            {
                return BadRequest(new { message = "Item ID mismatch." });
            }

            var existingItem = await _dbContext.Items.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound(new { message = "Item not found." });
            }

            // Update fields
            existingItem.Item_Name = updatedItem.Item_Name;
            existingItem.Description = updatedItem.Description;
            existingItem.Quantity = updatedItem.Quantity;
            existingItem.Price_in = updatedItem.Price_in;
            existingItem.Price_out = updatedItem.Price_out;
            existingItem.Discount = updatedItem.Discount;
            existingItem.Rate = updatedItem.Rate;
            existingItem.Category_ID = updatedItem.Category_ID;
            existingItem.Seller_ID = updatedItem.Seller_ID;
            existingItem.Sub_Category_ID = updatedItem.Sub_Category_ID;

            // Save changes
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the item.", error = ex.Message });
            }

            return Ok(new { message = "Item updated successfully." });
        }
        //public ActionResult UpdateProduct(Product product)
        //{
        //    var existingProduct = _dbContext.Set<Product>().Find(product.Item_ID);
        //    existingProduct.Item_Name = product.Item_Name;
        //    existingProduct.Description = product.Description;
        //    existingProduct.Quantity = product.Quantity;
        //    existingProduct.Price_in = product.Price_in;
        //    existingProduct.Price_out = product.Price_out;
        //    existingProduct.Discount = product.Discount;
        //    existingProduct.Rate = product.Rate;
        //    existingProduct.Category_ID = product.Category_ID;
        //    existingProduct.Sub_Category_ID = product.Sub_Category_ID;
        //    existingProduct.Seller_ID = product.Seller_ID;
        //    _dbContext.Set<Product>().Update(existingProduct);
        //    _dbContext.SaveChanges();
        //    return Ok("Updated");



        //}


        //public ActionResult RemoveProduct(int id)
        //{
        //    var existingProduct = _dbContext.Set<Product>().Find(id);
        //    _dbContext.Set<Product>().Remove(existingProduct);
        //    _dbContext.SaveChanges();
        //    return Ok("Removed");

        //}
        [HttpDelete]
        [Route("RemoveProduct{id}")]
        public async Task<IActionResult> DeleteItem(string id)
        {
            var item = await _dbContext.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound(new { message = "Item not found." });
            }

            _dbContext.Items.Remove(item);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Item deleted successfully." });
        }

    }
}
