using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public ReportsController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }


        [HttpGet("GetSellerNumbers")]
        public async Task<IActionResult> GetSellerNumbers(int Seller_ID)
        {
            var ordersList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetSellerNumbers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

        /// =================Views==================

        [HttpGet("GetMostViewedProductsBySeller")]
        public async Task<IActionResult> GetMostViewedProductsBySeller(int Seller_ID, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetMostViewedProductsBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);
                        command.Parameters.AddWithValue("@StartDate", (object?)StartDate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@EndDate", (object?)EndDate ?? DBNull.Value);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetDailyViewsForProductBySeller")]
        public async Task<IActionResult> GetDailyViewsForProductBySeller(int Seller_ID, string Item_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetDailyViewsForProductBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);
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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTopViewersBySeller")]
        public async Task<IActionResult> GetTopViewersBySeller(int Seller_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetTopViewersBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        /// =================products==================

        [HttpGet("GetTopProfitItemsBySeller")]
        public async Task<IActionResult> GetTopProfitItemsBySeller(int Seller_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetTopProfitItemsBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetLowestRatedItemsBySeller")]
        public async Task<IActionResult> GetLowestRatedItemsBySeller(int Seller_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetLowestRatedItemsBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetUnsoldItemsBySeller")]
        public async Task<IActionResult> GetUnsoldItemsBySeller(int Seller_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetUnsoldItemsBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetOutOfStockItemsBySeller")]
        public async Task<IActionResult> GetOutOfStockItemsBySeller(int Seller_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetOutOfStockItemsBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetDiscountedItemsBySeller")]
        public async Task<IActionResult> GetDiscountedItemsBySeller(int Seller_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetDiscountedItemsBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTopWishlistedItemsBySeller")]
        public async Task<IActionResult> GetTopWishlistedItemsBySeller(int Seller_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetTopWishlistedItemsBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTopComparedItemsBySeller")]
        public async Task<IActionResult> GetTopComparedItemsBySeller(int Seller_ID)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetTopComparedItemsBySeller", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

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

                return Ok(new { message = "success", data = resultList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// =================products==================

        [HttpGet("GetSellerOrders")]
        public async Task<IActionResult> GetSellerOrders(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerOrders", Seller_ID);
        }

        [HttpGet("GetSellerCompletedOrders")]
        public async Task<IActionResult> GetSellerCompletedOrders(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerCompletedOrders", Seller_ID);
        }

        [HttpGet("GetSellerOrdersByDateGroup")]
        public async Task<IActionResult> GetSellerOrdersByDateGroup(int Seller_ID, string GroupType)
        {
            return await ExecuteProcedure("GetSellerOrdersByDateGroup", Seller_ID, ("@GroupType", GroupType));
        }

        [HttpGet("GetSellerTopOrderedProducts")]
        public async Task<IActionResult> GetSellerTopOrderedProducts(int Seller_ID, int TopN = 10)
        {
            return await ExecuteProcedure("GetSellerTopOrderedProducts", Seller_ID, ("@TopN", TopN));
        }

        [HttpGet("GetSellerCancelledOrders")]
        public async Task<IActionResult> GetSellerCancelledOrders(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerCancelledOrders", Seller_ID);
        }

        [HttpGet("GetSellerOrdersByCity")]
        public async Task<IActionResult> GetSellerOrdersByCity(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerOrdersByCity", Seller_ID);
        }

        [HttpGet("GetSellerProfitByPeriod")]
        public async Task<IActionResult> GetSellerProfitByPeriod(int Seller_ID, string GroupType)
        {
            return await ExecuteProcedure("GetSellerProfitByPeriod", Seller_ID, ("@GroupType", GroupType));
        }

        [HttpGet("GetSellerProfitPerProduct")]
        public async Task<IActionResult> GetSellerProfitPerProduct(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerProfitPerProduct", Seller_ID);
        }

        [HttpGet("GetSellerTotalProfit")]
        public async Task<IActionResult> GetSellerTotalProfit(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerTotalProfit", Seller_ID);
        }

        [HttpGet("GetSellerProfitByCategory")]
        public async Task<IActionResult> GetSellerProfitByCategory(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerProfitByCategory", Seller_ID);
        }

        [HttpGet("GetSellerAverageRating")]
        public async Task<IActionResult> GetSellerAverageRating(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerAverageRating", Seller_ID);
        }

        [HttpGet("GetSellerRatingCount")]
        public async Task<IActionResult> GetSellerRatingCount(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerRatingCount", Seller_ID);
        }

        [HttpGet("GetLatestCommentsOnSellerProducts")]
        public async Task<IActionResult> GetLatestCommentsOnSellerProducts(int Seller_ID, int TopN = 10)
        {
            return await ExecuteProcedure("GetLatestCommentsOnSellerProducts", Seller_ID, ("@TopN", TopN));
        }

        [HttpGet("GetTopBottomRatedProducts")]
        public async Task<IActionResult> GetTopBottomRatedProducts(int Seller_ID, string Type, int TopN = 5)
        {
            return await ExecuteProcedure("GetTopBottomRatedProducts", Seller_ID, ("@Type", Type), ("@TopN", TopN));
        }

        [HttpGet("GetSellerTotalPayments")]
        public async Task<IActionResult> GetSellerTotalPayments(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerTotalPayments", Seller_ID);
        }

        [HttpGet("GetSellerPaymentByMonth")]
        public async Task<IActionResult> GetSellerPaymentByMonth(int Seller_ID)
        {
            return await ExecuteProcedure("GetSellerPaymentByMonth", Seller_ID);
        }

        [HttpGet("GetCartToPurchaseRate")]
        public async Task<IActionResult> GetCartToPurchaseRate(int Seller_ID)
        {
            return await ExecuteProcedure("GetCartToPurchaseRate", Seller_ID);
        }

        [HttpGet("GetWishlistToPurchaseRate")]
        public async Task<IActionResult> GetWishlistToPurchaseRate(int Seller_ID)
        {
            return await ExecuteProcedure("GetWishlistToPurchaseRate", Seller_ID);
        }

        [HttpGet("GetViewToBuyConversion")]
        public async Task<IActionResult> GetViewToBuyConversion(int Seller_ID)
        {
            return await ExecuteProcedure("GetViewToBuyConversion", Seller_ID);
        }

        [HttpGet("GetIgnoredProducts")]
        public async Task<IActionResult> GetIgnoredProducts(int Seller_ID)
        {
            return await ExecuteProcedure("GetIgnoredProducts", Seller_ID);
        }

        // 🔁 Helper function to execute stored procedures consistently
        private async Task<IActionResult> ExecuteProcedure(string procedureName, int Seller_ID, params (string, object)[] additionalParams)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Seller_ID", Seller_ID);

                        foreach (var (name, value) in additionalParams)
                        {
                            command.Parameters.AddWithValue(name, value);
                        }

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }

                                result.Add(row);
                            }
                        }
                    }
                }

                return Ok(new { message = "success", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
