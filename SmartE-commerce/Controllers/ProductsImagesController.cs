using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartE_commerce.Data;
using SmartE_commerce.Dto;

namespace SmartE_commerce.Controllers
{
    [Route("Products")]
    [ApiController]
    [Authorize]

    public class ProductsImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public ProductsImagesController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }


        [HttpPost]
        [Route("PostProductImage")]

        public async Task<IActionResult> AddItemImage([FromForm] ItemImageDto itemImageDto)
        {
            try
            {
                string imagePath = "Image_Cover.jpg"; // القيمة الافتراضية

                if (itemImageDto.Image != null)
                {
                    // 1. توليد اسم فريد للصورة
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(itemImageDto.Image.FileName);

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
                        await itemImageDto.Image.CopyToAsync(fileStream);
                    }

                    // 4. حفظ مسار الصورة
                    imagePath = $"/uploads/{fileName}";
                }

                // إضافة العنصر إلى قاعدة البيانات
                var itemImage = new ItemImage
                {
                    Item_ID = itemImageDto.Item_ID,

                    Item_Image = imagePath
                };

                _dbContext.ItemImages.Add(itemImage);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Item Image added successfully!" });
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

    }
}
