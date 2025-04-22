using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartE_commerce.Data;
using SmartE_commerce.Dto;
using System.Data;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Products")]
    [Authorize]

    public class ProductsControllerV2 : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString;

        public ProductsControllerV2(IWebHostEnvironment environment, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _environment = environment;
            _dbContext = dbContext;        
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }
        [HttpPost]
        [Route("PostProduct")]

        public async Task<IActionResult> AddItem([FromForm] ItemDto? itemDto)
        {
            try
            {
                string imagePath = "Image_Cover.jpg"; // القيمة الافتراضية

                if (itemDto.Image != null)
                {
                    // 1. توليد اسم فريد للصورة
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(itemDto.Image.FileName);

                    // 2. مسار الصورة
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // 3. حفظ الصورة
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await itemDto.Image.CopyToAsync(fileStream);
                    }

                    // 4. حفظ مسار الصورة
                    imagePath = $"/uploads/{fileName}";
                }

                // إضافة العنصر إلى قاعدة البيانات
                var item = new Item
                {
                    Item_ID = itemDto.Item_ID,
                    Item_Name = itemDto.Item_Name,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    Price_in = itemDto.Price_in,
                    Price_out = itemDto.Price_out,
                    Discount = itemDto.Discount,
                    Rate = itemDto.Rate,
                    Category_ID = itemDto.Category_ID,
                    Seller_ID = itemDto.Seller_ID,
                    Sub_Category_ID = itemDto.Sub_Category_ID,
                    Image_Cover = imagePath,
                    View_Count = 0,
                    Sold_Count = 0,
                    Brand_ID = itemDto.Brand_ID,
                    Crate_Date = DateTime.Now
                    
                };

                _dbContext.Items.Add(item);
                await _dbContext.SaveChangesAsync();

                var response = new Dictionary<string, object>();
                response["messageToUser"] = $"Item {item.Item_ID} added Successfully";

                response["message"] = "success";

                return Ok(response);

               
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while saving the entity changes.",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }


        [HttpPut]
        [Route("UpdateProductImage")]
        public async Task<IActionResult> UpdateProductImage(string ItemId, IFormFile Itemimage)
        {

            try
            {
                string imagePath = "Image_Cover.jpg"; // القيمة الافتراضية

                if (Itemimage != null)
                {
                    // 1. توليد اسم فريد للصورة
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Itemimage.FileName);

                    // 2. مسار الصورة
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // 3. حفظ الصورة
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Itemimage.CopyToAsync(fileStream);
                    }

                    // 4. حفظ مسار الصورة
                    imagePath = $"/uploads/{fileName}";
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_UpdateItemImagecoverv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ItemID", ItemId);
                        command.Parameters.AddWithValue("@ImageCover", imagePath);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                var response = new Dictionary<string, object>();
                response["messageToUser"] = $"Item {ItemId} Image added Successfully";
                response["message"] = "success";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while saving the entity changes.",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        //[HttpPut]
        //[Route("UpdateProductImageUrl")]
        //public async Task<IActionResult> UpdateProductImageUrl(string ItemId, string Itemimageurl)
        //{

        //    try
        //    {
        //        string imagePath = "Image_Cover.jpg"; // القيمة الافتراضية

        //        if (Itemimage != null)
        //        {
        //            // 1. توليد اسم فريد للصورة
        //            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Itemimage.FileName);

        //            // 2. مسار الصورة
        //            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }
        //            var filePath = Path.Combine(uploadsFolder, fileName);

        //            // 3. حفظ الصورة
        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await Itemimage.CopyToAsync(fileStream);
        //            }

        //            // 4. حفظ مسار الصورة
        //            imagePath = $"/uploads/{fileName}";
        //        }

        //        using (SqlConnection connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            using (SqlCommand command = new SqlCommand("Sp_UpdateItemImagecoverv4", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.AddWithValue("@ItemID", ItemId);
        //                command.Parameters.AddWithValue("@ImageCover", imagePath);
        //                await command.ExecuteNonQueryAsync();
        //            }
        //        }
        //        var response = new Dictionary<string, object>();
        //        response["messageToUser"] = $"Item {ItemId} Image added Successfully";
        //        response["message"] = "success";
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            message = "An error occurred while saving the entity changes.",
        //            error = ex.Message,
        //            innerException = ex.InnerException?.Message
        //        });
        //    }
        //}

    }
}
