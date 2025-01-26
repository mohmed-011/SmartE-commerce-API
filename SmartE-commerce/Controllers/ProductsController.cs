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
        private readonly string _connectionString = "server=.;database=Smart_EcommerceV4;integrated security =true; trust server certificate = true ";


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

                    using (SqlCommand command = new SqlCommand("Sp_GetAllProductsv4", connection))
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

                    using (SqlCommand command = new SqlCommand("Sp_GetProductByIdv4", connection))
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
