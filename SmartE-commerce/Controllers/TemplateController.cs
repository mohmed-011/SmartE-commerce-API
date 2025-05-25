using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        [HttpGet("downloadLaptops")]
        public IActionResult DownloadTemplateLaptops()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Laptops_Test_Data.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Laptops_Test_Data.xlsx");
        }



        [HttpGet("downloadPCs")]
        public IActionResult DownloadTemplatePCs()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "PCs_Data - Copy.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "PCs_Data - Copy.xlsx");
        }



        [HttpGet("downloadPhones")]
        public IActionResult DownloadTemplatePhones()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Phones_Test_Data - Copy.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Phones_Test_Data - Copy.xlsx");
        }



        [HttpGet("downloadSmartWatches")]
        public IActionResult DownloadTemplateSmartWatches()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Smart_Watches Data.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Smart_Watches Data.xlsx");
        }



        [HttpGet("downloadTVs")]
        public IActionResult DownloadTemplateTVs()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "TVs Test_Data - Copy.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Template file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "TVs Test_Data - Copy");
        }
    }
}
