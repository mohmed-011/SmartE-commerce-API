using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartE_commerce.Data;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Products")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString = "server=.;database=Smart_EcommerceV2;integrated security =true; trust server certificate = true ";


        public ProductsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetAllProducts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


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
        [Route("GetProductById{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var resultList = new List<Dictionary<string, object>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_GetProductById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ItemID", id);


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
        public ActionResult UpdateProduct(Product product)
        {
            var existingProduct = _dbContext.Set<Product>().Find(product.Item_ID);
            existingProduct.Item_Name = product.Item_Name;
            existingProduct.Description = product.Description;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Price_in = product.Price_in;
            existingProduct.Price_out = product.Price_out;
            existingProduct.Discount = product.Discount;
            existingProduct.Rate = product.Rate;
            existingProduct.Category_ID = product.Category_ID;
            existingProduct.Sub_Category_ID = product.Sub_Category_ID;
            existingProduct.Seller_ID = product.Seller_ID;
            _dbContext.Set<Product>().Update(existingProduct);
            _dbContext.SaveChanges();
            return Ok("Updated");



        }

        [HttpDelete]
        [Route("RemoveProduct{id}")]
        public ActionResult RemoveProduct(int id)
        {
            var existingProduct = _dbContext.Set<Product>().Find(id);
            _dbContext.Set<Product>().Remove(existingProduct);
            _dbContext.SaveChanges();
            return Ok("Removed");

        }

    }
}
