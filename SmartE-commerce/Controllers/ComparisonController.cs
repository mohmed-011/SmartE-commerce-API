using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Comparison")]
    [Authorize]

    public class ComparisonController : ControllerBase
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public ComparisonController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }


        [HttpGet("GetTwoItemsComp")]
        public async Task<IActionResult> GetTwoItemsComp(string id1 , string id2)
        {
            var ordersList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetTwoItemsMonthlySalesDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ItemOneID", id1);
                        command.Parameters.AddWithValue("@ItemTwoID", id2);


                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }

                                ordersList.Add(row);
                            }
                        }
                    }
                }

                var response = new
                {
                    message = "success",
                    data = ordersList
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }








        [HttpGet("GetAllComparisons")]
        public async Task<IActionResult> GetAllComparisons(int BuyerId)
        {
            try
            {
                var tasks = new List<Task<Dictionary<string, object>>>()
            {
                GetComparisonData("Sp_GetPhonesComparisonv4", BuyerId),
                GetComparisonData("Sp_GetLaptopsComparisonv4", BuyerId),
                GetComparisonData("Sp_GetSmart_WatchesComparisonv4", BuyerId),
                GetComparisonData("Sp_GetTVsComparisonv4", BuyerId),
                GetComparisonData("Sp_GetPCsComparisonv4", BuyerId)
            };

                var results = await Task.WhenAll(tasks);

                return Ok(new
                {
                    Phones = results[0],
                    Laptops = results[1],
                    SmartWatches = results[2],
                    TVs = results[3],
                    PCs = results[4]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<Dictionary<string, object>> GetComparisonData(string storedProcedure, int BuyerId)
        {
            var resultList = new List<Dictionary<string, object>>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Buyer_ID", BuyerId);

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
                                    imagesData = await GetItemImages(connection, itemId);
                                    row["images"] = imagesData;
                                }
                            }

                            resultList.Add(row);
                        }
                    }
                }
            }

            return new Dictionary<string, object> { { "Data", resultList } };
        }

        private async Task<Dictionary<string, string>> GetItemImages(SqlConnection connection, string itemId)
        {
            var imagesData = new Dictionary<string, string>();

            using (SqlCommand command = new SqlCommand("Sp_GetItemImagesv4", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ItemID", itemId);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    int j = 1;
                    while (await reader.ReadAsync())
                    {
                        imagesData[$"Item_Images-{j}"] = reader.GetValue(0).ToString();
                        j++;
                    }
                }
            }

            return imagesData;
        }



        [HttpGet("GetPhonesComparison")]
        public async Task<IActionResult> GetPhonesComparison(string BuyerId)
        {
            var resultList = new List<Dictionary<string, object>>(); // List to store each product's data

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetPhonesComparisonv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Buyer_ID", BuyerId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                var imagesData = new Dictionary<string, string>(); // Dictionary to store images for each product

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                    if (reader.GetName(i) == "Item_ID") // Once Item_ID is found
                                    {
                                        var itemId = reader.GetValue(i).ToString();

                                        // Now fetch the images for this Item_ID
                                        using (SqlCommand command2 = new SqlCommand("Sp_GetItemImagesv4", connection))
                                        {
                                            command2.CommandType = CommandType.StoredProcedure;
                                            command2.Parameters.AddWithValue("@ItemID", itemId);

                                            using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                                            {
                                                int j = 1;
                                                while (await reader2.ReadAsync())
                                                {
                                                    // Add images to the dictionary with a dynamic key
                                                    imagesData[$"Item_Images-{j}"] = reader2.GetValue(0).ToString();
                                                    j++;
                                                }
                                            }
                                        }
                                    }
                                }

                                // Add imagesData to the current product row
                                if (imagesData.Count > 0)
                                {
                                    row["images"] = imagesData;
                                }

                                // Add the product row to the result list
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
        [HttpGet("GetLaptopsComparison")]
        public async Task<IActionResult> GetLaptopsComparison(int BuyerId)
        {
            var resultList = new List<Dictionary<string, object>>(); // List to store each product's data

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetLaptopsComparisonv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Buyer_ID", BuyerId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                var imagesData = new Dictionary<string, string>(); // Dictionary to store images for each product

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                    if (reader.GetName(i) == "Item_ID") // Once Item_ID is found
                                    {
                                        var itemId = reader.GetValue(i).ToString();

                                        // Now fetch the images for this Item_ID
                                        using (SqlCommand command2 = new SqlCommand("Sp_GetItemImagesv4", connection))
                                        {
                                            command2.CommandType = CommandType.StoredProcedure;
                                            command2.Parameters.AddWithValue("@ItemID", itemId);

                                            using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                                            {
                                                int j = 1;
                                                while (await reader2.ReadAsync())
                                                {
                                                    // Add images to the dictionary with a dynamic key
                                                    imagesData[$"Item_Images-{j}"] = reader2.GetValue(0).ToString();
                                                    j++;
                                                }
                                            }
                                        }
                                    }
                                }

                                // Add imagesData to the current product row
                                if (imagesData.Count > 0)
                                {
                                    row["images"] = imagesData;
                                }

                                // Add the product row to the result list
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
        [HttpGet("GetSmartWatchesComparison")]
        public async Task<IActionResult> GetSmartWatchesComparison(int BuyerId)
        {
            var resultList = new List<Dictionary<string, object>>(); // List to store each product's data

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetSmart_WatchesComparisonv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Buyer_ID", BuyerId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                var imagesData = new Dictionary<string, string>(); // Dictionary to store images for each product

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                    if (reader.GetName(i) == "Item_ID") // Once Item_ID is found
                                    {
                                        var itemId = reader.GetValue(i).ToString();

                                        // Now fetch the images for this Item_ID
                                        using (SqlCommand command2 = new SqlCommand("Sp_GetItemImagesv4", connection))
                                        {
                                            command2.CommandType = CommandType.StoredProcedure;
                                            command2.Parameters.AddWithValue("@ItemID", itemId);

                                            using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                                            {
                                                int j = 1;
                                                while (await reader2.ReadAsync())
                                                {
                                                    // Add images to the dictionary with a dynamic key
                                                    imagesData[$"Item_Images-{j}"] = reader2.GetValue(0).ToString();
                                                    j++;
                                                }
                                            }
                                        }
                                    }
                                }

                                // Add imagesData to the current product row
                                if (imagesData.Count > 0)
                                {
                                    row["images"] = imagesData;
                                }

                                // Add the product row to the result list
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
        [HttpGet("GetTVsComparison")]
        public async Task<IActionResult> GetTVsComparison(int BuyerId)
        {
            var resultList = new List<Dictionary<string, object>>(); // List to store each product's data

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetTVsComparisonv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Buyer_ID", BuyerId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                var imagesData = new Dictionary<string, string>(); // Dictionary to store images for each product

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                    if (reader.GetName(i) == "Item_ID") // Once Item_ID is found
                                    {
                                        var itemId = reader.GetValue(i).ToString();

                                        // Now fetch the images for this Item_ID
                                        using (SqlCommand command2 = new SqlCommand("Sp_GetItemImagesv4", connection))
                                        {
                                            command2.CommandType = CommandType.StoredProcedure;
                                            command2.Parameters.AddWithValue("@ItemID", itemId);

                                            using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                                            {
                                                int j = 1;
                                                while (await reader2.ReadAsync())
                                                {
                                                    // Add images to the dictionary with a dynamic key
                                                    imagesData[$"Item_Images-{j}"] = reader2.GetValue(0).ToString();
                                                    j++;
                                                }
                                            }
                                        }
                                    }
                                }

                                // Add imagesData to the current product row
                                if (imagesData.Count > 0)
                                {
                                    row["images"] = imagesData;
                                }

                                // Add the product row to the result list
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
        [HttpGet("GetPCsComparison")]
        public async Task<IActionResult> GetPCsComparison(int BuyerId)
        {
            var resultList = new List<Dictionary<string, object>>(); // List to store each product's data

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetPCsComparisonv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Buyer_ID", BuyerId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                var imagesData = new Dictionary<string, string>(); // Dictionary to store images for each product

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                    if (reader.GetName(i) == "Item_ID") // Once Item_ID is found
                                    {
                                        var itemId = reader.GetValue(i).ToString();

                                        // Now fetch the images for this Item_ID
                                        using (SqlCommand command2 = new SqlCommand("Sp_GetItemImagesv4", connection))
                                        {
                                            command2.CommandType = CommandType.StoredProcedure;
                                            command2.Parameters.AddWithValue("@ItemID", itemId);

                                            using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                                            {
                                                int j = 1;
                                                while (await reader2.ReadAsync())
                                                {
                                                    // Add images to the dictionary with a dynamic key
                                                    imagesData[$"Item_Images-{j}"] = reader2.GetValue(0).ToString();
                                                    j++;
                                                }
                                            }
                                        }
                                    }
                                }

                                // Add imagesData to the current product row
                                if (imagesData.Count > 0)
                                {
                                    row["images"] = imagesData;
                                }

                                // Add the product row to the result list
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



        [HttpDelete("RemoveFromComparisons")]
        public async Task<IActionResult> RemoveFromComparisons(string ItemId, int BuyerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_DeleteFromComparisonv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ItemID", ItemId);
                        command.Parameters.AddWithValue("@Buyer_ID", BuyerId);


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


        [HttpPost("AddToComparison")]
        public async Task<IActionResult> AddItem(string ItemId, int BuyerId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_AddToComparisonv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ItemID", ItemId);
                        command.Parameters.AddWithValue("@Buyer_ID", BuyerId);




                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok($"Item {ItemId} posted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
