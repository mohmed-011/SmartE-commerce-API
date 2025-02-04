using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SmartE_commerce.Controllers
{


    [Route("Excel")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly string _connectionString = "server=.;database=Smart_EcommerceV4;integrated security =true; trust server certificate = true ";

        [HttpPost("PostLaptops")]
        public async Task<IActionResult> PostLaptops([FromForm] IFormFile file,int SellerId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Temporarily save the file
                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Read data from Excel file
                var dataTable = ReadExcelFile(filePath);

                // Insert data into database
                await InsertDataIntoLaptops(dataTable , SellerId);

                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private async Task InsertDataIntoLaptops(DataTable dataTable , int SellerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (DataRow row in dataTable.Rows)
                {
                    int Item_ID = row["Item_ID"] != DBNull.Value ? Convert.ToInt32(row["Item_ID"]) : 0;
                    int Seller_ID = SellerId;

                    var command = new SqlCommand(
                @"INSERT INTO Item (
                    Item_ID, Image_Cover, Item_Name, Description, Quantity, 
                    Price_in, Price_out, Discount, Rate, Category_ID, 
                    Seller_ID, Sub_Category_ID
                ) 
                VALUES (
                    @Item_ID, @Image_Cover, @Item_Name, @Description, @Quantity, 
                    @Price_in, @Price_out, @Discount, @Rate, @Category_ID, 
                    @Seller_ID, @Sub_Category_ID
                )",
                connection);

                    // Mapping parameters
                    command.Parameters.AddWithValue("@Item_ID",( ""+Seller_ID + "-" + Item_ID+"" ));
                    command.Parameters.AddWithValue("@Image_Cover", row["Image_Cover"] != DBNull.Value && !string.IsNullOrEmpty(row["Image_Cover"].ToString()) ? row["Image_Cover"].ToString() : "Image_Cover.jpg");
                    command.Parameters.AddWithValue("@Item_Name", row["Item_Name"] != DBNull.Value ? row["Item_Name"].ToString() : string.Empty);
                    command.Parameters.AddWithValue("@Description", row["Description"] != DBNull.Value ? row["Description"].ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0);
                    command.Parameters.AddWithValue("@Price_in", row["Price_in"] != DBNull.Value ? Convert.ToDecimal(row["Price_in"]) : 0.0m);
                    command.Parameters.AddWithValue("@Price_out", row["Price_out"] != DBNull.Value ? Convert.ToDecimal(row["Price_out"]) : 0.0m);
                    command.Parameters.AddWithValue("@Discount", row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : DBNull.Value);
                    command.Parameters.AddWithValue("@Rate", 2.5);
                    command.Parameters.AddWithValue("@Category_ID", 1);
                    command.Parameters.AddWithValue("@Seller_ID", Seller_ID);
                    command.Parameters.AddWithValue("@Sub_Category_ID",2);

                    await command.ExecuteNonQueryAsync();

                    var insertLaptopCommand = new SqlCommand(
               @"INSERT INTO Laptops (
                    Item_ID, Brand, RAM, Memory, Memory_Type, 
                    Graphics_Card, CPU, Weight, Screen_Size, Model
                ) 
                VALUES (
                    @Item_ID, @Brand, @RAM, @Memory, @Memory_Type, 
                    @Graphics_Card, @CPU, @Weight, @Screen_Size, @Model
                )",
               connection);

                    // إعداد المعاملات لجدول Laptops
                    insertLaptopCommand.Parameters.AddWithValue("@Item_ID", "" + Seller_ID + "-" + Item_ID + "");
                    insertLaptopCommand.Parameters.AddWithValue("@Brand", row["Brand"] != DBNull.Value ? row["Brand"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@RAM", row["RAM"] != DBNull.Value ? Convert.ToInt32(row["RAM"]) : 0);
                    insertLaptopCommand.Parameters.AddWithValue("@Memory", row["Memory"] != DBNull.Value ? Convert.ToInt32(row["Memory"]) : 0);
                    insertLaptopCommand.Parameters.AddWithValue("@Memory_Type", row["Memory_Type"] != DBNull.Value ? row["Memory_Type"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@Graphics_Card", row["Graphics_Card"] != DBNull.Value ? row["Graphics_Card"].ToString() : DBNull.Value);
                    insertLaptopCommand.Parameters.AddWithValue("@CPU", row["CPU"] != DBNull.Value ? row["CPU"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@Weight", row["Weight"] != DBNull.Value ? Convert.ToDecimal(row["Weight"]) : DBNull.Value);
                    insertLaptopCommand.Parameters.AddWithValue("@Screen_Size", row["Screen_Size"] != DBNull.Value ? Convert.ToDecimal(row["Screen_Size"]) : DBNull.Value);
                    insertLaptopCommand.Parameters.AddWithValue("@Model", row["Model"] != DBNull.Value ? row["Model"].ToString() : DBNull.Value);

                    await insertLaptopCommand.ExecuteNonQueryAsync();
                }
            }
        }



        [HttpPost("PostPCs")]
        public async Task<IActionResult> PostPCs([FromForm] IFormFile file, int SellerId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Temporarily save the file
                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Read data from Excel file
                var dataTable = ReadExcelFile(filePath);

                // Insert data into database
                await InsertDataIntoPCs(dataTable, SellerId);

                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private async Task InsertDataIntoPCs(DataTable dataTable, int SellerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (DataRow row in dataTable.Rows)
                {
                    int Item_ID = row["Item_ID"] != DBNull.Value ? Convert.ToInt32(row["Item_ID"]) : 0;
                    int Seller_ID = SellerId;

                    var command = new SqlCommand(
                @"INSERT INTO Item (
                    Item_ID, Image_Cover, Item_Name, Description, Quantity, 
                    Price_in, Price_out, Discount, Rate, Category_ID, 
                    Seller_ID, Sub_Category_ID
                ) 
                VALUES (
                    @Item_ID, @Image_Cover, @Item_Name, @Description, @Quantity, 
                    @Price_in, @Price_out, @Discount, @Rate, @Category_ID, 
                    @Seller_ID, @Sub_Category_ID
                )",
                connection);

                    // Mapping parameters
                    command.Parameters.AddWithValue("@Item_ID", ("" + Seller_ID + "-" + Item_ID + ""));
                    command.Parameters.AddWithValue("@Image_Cover", row["Image_Cover"] != DBNull.Value && !string.IsNullOrEmpty(row["Image_Cover"].ToString()) ? row["Image_Cover"].ToString() : "Image_Cover.jpg");
                    command.Parameters.AddWithValue("@Item_Name", row["Item_Name"] != DBNull.Value ? row["Item_Name"].ToString() : string.Empty);
                    command.Parameters.AddWithValue("@Description", row["Description"] != DBNull.Value ? row["Description"].ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0);
                    command.Parameters.AddWithValue("@Price_in", row["Price_in"] != DBNull.Value ? Convert.ToDecimal(row["Price_in"]) : 0.0m);
                    command.Parameters.AddWithValue("@Price_out", row["Price_out"] != DBNull.Value ? Convert.ToDecimal(row["Price_out"]) : 0.0m);
                    command.Parameters.AddWithValue("@Discount", row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : DBNull.Value);
                    command.Parameters.AddWithValue("@Rate", 2.5);
                    command.Parameters.AddWithValue("@Category_ID", 1);
                    command.Parameters.AddWithValue("@Seller_ID", Seller_ID);
                    command.Parameters.AddWithValue("@Sub_Category_ID", 19);

                    await command.ExecuteNonQueryAsync();

                    var insertLaptopCommand = new SqlCommand(
               @"INSERT INTO PCs (
                    Item_ID, Brand, RAM, Memory, Memory_Type, 
                    Graphics_Card, CPU
                ) 
                VALUES (
                    @Item_ID, @Brand, @RAM, @Memory, @Memory_Type, 
                    @Graphics_Card, @CPU
                )",
               connection);

                    // إعداد المعاملات لجدول Laptops
                    insertLaptopCommand.Parameters.AddWithValue("@Item_ID", "" + Seller_ID + "-" + Item_ID + "");
                    insertLaptopCommand.Parameters.AddWithValue("@Brand", row["Brand"] != DBNull.Value ? row["Brand"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@RAM", row["RAM"] != DBNull.Value ? Convert.ToInt32(row["RAM"]) : 0);
                    insertLaptopCommand.Parameters.AddWithValue("@Memory", row["Memory"] != DBNull.Value ? Convert.ToInt32(row["Memory"]) : 0);
                    insertLaptopCommand.Parameters.AddWithValue("@Memory_Type", row["Memory_Type"] != DBNull.Value ? row["Memory_Type"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@Graphics_Card", row["Graphics_Card"] != DBNull.Value ? row["Graphics_Card"].ToString() : DBNull.Value);
                    insertLaptopCommand.Parameters.AddWithValue("@CPU", row["CPU"] != DBNull.Value ? row["CPU"].ToString() : string.Empty);
                   
                    await insertLaptopCommand.ExecuteNonQueryAsync();
                }
            }
        }



        [HttpPost("PostPhones")]
        public async Task<IActionResult> PostPhones([FromForm] IFormFile file, int SellerId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Temporarily save the file
                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Read data from Excel file
                var dataTable = ReadExcelFile(filePath);

                // Insert data into database
                await InsertDataIntoPhones(dataTable, SellerId);

                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private async Task InsertDataIntoPhones(DataTable dataTable, int SellerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (DataRow row in dataTable.Rows)
                {
                    int Item_ID = row["Item_ID"] != DBNull.Value ? Convert.ToInt32(row["Item_ID"]) : 0;
                    int Seller_ID = SellerId;

                    var command = new SqlCommand(
                @"INSERT INTO Item (
                    Item_ID, Image_Cover, Item_Name, Description, Quantity, 
                    Price_in, Price_out, Discount, Rate, Category_ID, 
                    Seller_ID, Sub_Category_ID
                ) 
                VALUES (
                    @Item_ID, @Image_Cover, @Item_Name, @Description, @Quantity, 
                    @Price_in, @Price_out, @Discount, @Rate, @Category_ID, 
                    @Seller_ID, @Sub_Category_ID
                )",
                connection);

                    // Mapping parameters
                    command.Parameters.AddWithValue("@Item_ID", ("" + Seller_ID + "-" + Item_ID + ""));
                    command.Parameters.AddWithValue("@Image_Cover", row["Image_Cover"] != DBNull.Value && !string.IsNullOrEmpty(row["Image_Cover"].ToString()) ? row["Image_Cover"].ToString() : "Image_Cover.jpg");
                    command.Parameters.AddWithValue("@Item_Name", row["Item_Name"] != DBNull.Value ? row["Item_Name"].ToString() : string.Empty);
                    command.Parameters.AddWithValue("@Description", row["Description"] != DBNull.Value ? row["Description"].ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0);
                    command.Parameters.AddWithValue("@Price_in", row["Price_in"] != DBNull.Value ? Convert.ToDecimal(row["Price_in"]) : 0.0m);
                    command.Parameters.AddWithValue("@Price_out", row["Price_out"] != DBNull.Value ? Convert.ToDecimal(row["Price_out"]) : 0.0m);
                    command.Parameters.AddWithValue("@Discount", row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : DBNull.Value);
                    command.Parameters.AddWithValue("@Rate", 2.5);
                    command.Parameters.AddWithValue("@Category_ID", 1);
                    command.Parameters.AddWithValue("@Seller_ID", Seller_ID);
                    command.Parameters.AddWithValue("@Sub_Category_ID", 1);

                    await command.ExecuteNonQueryAsync();

                    var insertLaptopCommand = new SqlCommand(
               @"INSERT INTO Phones (
                    Item_ID, Brand, RAM, Memory, 
                     CPU , Color , Screen_Size
                ) 
                VALUES (
                    @Item_ID, @Brand, @RAM, @Memory,
                     @CPU , @Color , @Screen_Size
                )",
               connection);

                    // إعداد المعاملات لجدول Laptops
                    insertLaptopCommand.Parameters.AddWithValue("@Item_ID", "" + Seller_ID + "-" + Item_ID + "");
                    insertLaptopCommand.Parameters.AddWithValue("@Brand", row["Brand"] != DBNull.Value ? row["Brand"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@RAM", row["RAM"] != DBNull.Value ? Convert.ToInt32(row["RAM"]) : 0);
                    insertLaptopCommand.Parameters.AddWithValue("@Memory", row["Memory"] != DBNull.Value ? Convert.ToInt32(row["Memory"]) : 0);
                    insertLaptopCommand.Parameters.AddWithValue("@CPU", row["CPU"] != DBNull.Value ? row["CPU"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@Color", row["Color"] != DBNull.Value ? row["Color"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@Screen_Size", row["Screen_Size"] != DBNull.Value ? Convert.ToDecimal(row["Screen_Size"]) : DBNull.Value);

                    await insertLaptopCommand.ExecuteNonQueryAsync();
                }
            }
        }




        [HttpPost("PostSmartWatches")]
        public async Task<IActionResult> PostSmartWatches([FromForm] IFormFile file, int SellerId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Temporarily save the file
                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Read data from Excel file
                var dataTable = ReadExcelFile(filePath);

                // Insert data into database
                await InsertDataIntoSmartWatches(dataTable, SellerId);

                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private async Task InsertDataIntoSmartWatches(DataTable dataTable, int SellerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (DataRow row in dataTable.Rows)
                {
                    int Item_ID = row["Item_ID"] != DBNull.Value ? Convert.ToInt32(row["Item_ID"]) : 0;
                    int Seller_ID = SellerId;

                    var command = new SqlCommand(
                @"INSERT INTO Item (
                    Item_ID, Image_Cover, Item_Name, Description, Quantity, 
                    Price_in, Price_out, Discount, Rate, Category_ID, 
                    Seller_ID, Sub_Category_ID
                ) 
                VALUES (
                    @Item_ID, @Image_Cover, @Item_Name, @Description, @Quantity, 
                    @Price_in, @Price_out, @Discount, @Rate, @Category_ID, 
                    @Seller_ID, @Sub_Category_ID
                )",
                connection);

                    // Mapping parameters
                    command.Parameters.AddWithValue("@Item_ID", ("" + Seller_ID + "-" + Item_ID + ""));
                    command.Parameters.AddWithValue("@Image_Cover", row["Image_Cover"] != DBNull.Value && !string.IsNullOrEmpty(row["Image_Cover"].ToString()) ? row["Image_Cover"].ToString() : "Image_Cover.jpg");
                    command.Parameters.AddWithValue("@Item_Name", row["Item_Name"] != DBNull.Value ? row["Item_Name"].ToString() : string.Empty);
                    command.Parameters.AddWithValue("@Description", row["Description"] != DBNull.Value ? row["Description"].ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0);
                    command.Parameters.AddWithValue("@Price_in", row["Price_in"] != DBNull.Value ? Convert.ToDecimal(row["Price_in"]) : 0.0m);
                    command.Parameters.AddWithValue("@Price_out", row["Price_out"] != DBNull.Value ? Convert.ToDecimal(row["Price_out"]) : 0.0m);
                    command.Parameters.AddWithValue("@Discount", row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : DBNull.Value);
                    command.Parameters.AddWithValue("@Rate", 2.5);
                    command.Parameters.AddWithValue("@Category_ID", 1);
                    command.Parameters.AddWithValue("@Seller_ID", Seller_ID);
                    command.Parameters.AddWithValue("@Sub_Category_ID", 4);

                    await command.ExecuteNonQueryAsync();

                    var insertLaptopCommand = new SqlCommand(
               @"INSERT INTO Smart_Watches (
                    Item_ID, Brand ,Model, Screen_Size , OS , Color
                ) 
                VALUES (
                    @Item_ID, @Brand ,@Model, @Screen_Size , @OS , @Color
                )",
               connection);

                    // إعداد المعاملات لجدول Laptops
                    insertLaptopCommand.Parameters.AddWithValue("@Item_ID", "" + Seller_ID + "-" + Item_ID + "");
                    insertLaptopCommand.Parameters.AddWithValue("@Brand", row["Brand"] != DBNull.Value ? row["Brand"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@Model", row["Model"] != DBNull.Value ? row["Model"].ToString() : DBNull.Value);
                    insertLaptopCommand.Parameters.AddWithValue("@Screen_Size", row["Screen_Size"] != DBNull.Value ? Convert.ToDecimal(row["Screen_Size"]) : DBNull.Value);
                    insertLaptopCommand.Parameters.AddWithValue("@OS", row["OS"] != DBNull.Value ? row["OS"].ToString() : DBNull.Value);
                    insertLaptopCommand.Parameters.AddWithValue("@Color", row["Color"] != DBNull.Value ? row["Color"].ToString() : string.Empty);

                    await insertLaptopCommand.ExecuteNonQueryAsync();
                }
            }
        }



        [HttpPost("PostTVs")]
        public async Task<IActionResult> PostTVs([FromForm] IFormFile file, int SellerId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Temporarily save the file
                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Read data from Excel file
                var dataTable = ReadExcelFile(filePath);

                // Insert data into database
                await InsertDataIntoTVs(dataTable, SellerId);

                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private async Task InsertDataIntoTVs(DataTable dataTable, int SellerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (DataRow row in dataTable.Rows)
                {
                    int Item_ID = row["Item_ID"] != DBNull.Value ? Convert.ToInt32(row["Item_ID"]) : 0;
                    int Seller_ID = SellerId;

                    var command = new SqlCommand(
                @"INSERT INTO Item (
                    Item_ID, Image_Cover, Item_Name, Description, Quantity, 
                    Price_in, Price_out, Discount, Rate, Category_ID, 
                    Seller_ID, Sub_Category_ID
                ) 
                VALUES (
                    @Item_ID, @Image_Cover, @Item_Name, @Description, @Quantity, 
                    @Price_in, @Price_out, @Discount, @Rate, @Category_ID, 
                    @Seller_ID, @Sub_Category_ID
                )",
                connection);

                    // Mapping parameters
                    command.Parameters.AddWithValue("@Item_ID", ("" + Seller_ID + "-" + Item_ID + ""));
                    command.Parameters.AddWithValue("@Image_Cover", row["Image_Cover"] != DBNull.Value && !string.IsNullOrEmpty(row["Image_Cover"].ToString()) ? row["Image_Cover"].ToString() : "Image_Cover.jpg");
                    command.Parameters.AddWithValue("@Item_Name", row["Item_Name"] != DBNull.Value ? row["Item_Name"].ToString() : string.Empty);
                    command.Parameters.AddWithValue("@Description", row["Description"] != DBNull.Value ? row["Description"].ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0);
                    command.Parameters.AddWithValue("@Price_in", row["Price_in"] != DBNull.Value ? Convert.ToDecimal(row["Price_in"]) : 0.0m);
                    command.Parameters.AddWithValue("@Price_out", row["Price_out"] != DBNull.Value ? Convert.ToDecimal(row["Price_out"]) : 0.0m);
                    command.Parameters.AddWithValue("@Discount", row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : DBNull.Value);
                    command.Parameters.AddWithValue("@Rate", 2.5);
                    command.Parameters.AddWithValue("@Category_ID", 2);
                    command.Parameters.AddWithValue("@Seller_ID", Seller_ID);
                    command.Parameters.AddWithValue("@Sub_Category_ID", 7);

                    await command.ExecuteNonQueryAsync();

                    var insertLaptopCommand = new SqlCommand(
               @"INSERT INTO TVs (
                    Item_ID, Brand, Screen_Size , Resolution  ,Model
                ) 
                VALUES (
                    @Item_ID, @Brand , @Screen_Size , @Resolution ,@Model
                )",
               connection);

                    // إعداد المعاملات لجدول Laptops
                    insertLaptopCommand.Parameters.AddWithValue("@Item_ID", "" + Seller_ID + "-" + Item_ID + "");
                    insertLaptopCommand.Parameters.AddWithValue("@Brand", row["Brand"] != DBNull.Value ? row["Brand"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@Screen_Size", row["Screen_Size"] != DBNull.Value ? Convert.ToDecimal(row["Screen_Size"]) : DBNull.Value);
                    insertLaptopCommand.Parameters.AddWithValue("@Resolution", row["Resolution"] != DBNull.Value ? row["Resolution"].ToString() : string.Empty);
                    insertLaptopCommand.Parameters.AddWithValue("@Model", row["Model"] != DBNull.Value ? row["Model"].ToString() : DBNull.Value);

                    await insertLaptopCommand.ExecuteNonQueryAsync();
                }
            }
        }





        [HttpPost("PostItems")]
        public async Task<IActionResult> PostItem([FromForm] IFormFile file, int SellerId , int CategoryId , int SubCategoryId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Temporarily save the file
                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Read data from Excel file
                var dataTable = ReadExcelFile(filePath);

                // Insert data into database
                await InsertDataIntoItem(dataTable, SellerId,CategoryId,SubCategoryId);

                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private async Task InsertDataIntoItem(DataTable dataTable, int SellerId , int CategoryId, int SubCategoryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (DataRow row in dataTable.Rows)
                {
                    int Item_ID = row["Item_ID"] != DBNull.Value ? Convert.ToInt32(row["Item_ID"]) : 0;
                    int Seller_ID = SellerId;

                    var command = new SqlCommand(
                @"INSERT INTO Item (
                    Item_ID, Image_Cover, Item_Name, Description, Quantity, 
                    Price_in, Price_out, Discount, Rate, Category_ID, 
                    Seller_ID, Sub_Category_ID
                ) 
                VALUES (
                    @Item_ID, @Image_Cover, @Item_Name, @Description, @Quantity, 
                    @Price_in, @Price_out, @Discount, @Rate, @Category_ID, 
                    @Seller_ID, @Sub_Category_ID
                )",
                connection);

                    // Mapping parameters
                    command.Parameters.AddWithValue("@Item_ID", ("" + Seller_ID + "-" + Item_ID + ""));
                    command.Parameters.AddWithValue("@Image_Cover", row["Image_Cover"] != DBNull.Value && !string.IsNullOrEmpty(row["Image_Cover"].ToString()) ? row["Image_Cover"].ToString() : "Image_Cover.jpg");
                    command.Parameters.AddWithValue("@Item_Name", row["Item_Name"] != DBNull.Value ? row["Item_Name"].ToString() : string.Empty);
                    command.Parameters.AddWithValue("@Description", row["Description"] != DBNull.Value ? row["Description"].ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0);
                    command.Parameters.AddWithValue("@Price_in", row["Price_in"] != DBNull.Value ? Convert.ToDecimal(row["Price_in"]) : 0.0m);
                    command.Parameters.AddWithValue("@Price_out", row["Price_out"] != DBNull.Value ? Convert.ToDecimal(row["Price_out"]) : 0.0m);
                    command.Parameters.AddWithValue("@Discount", row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : DBNull.Value);
                    command.Parameters.AddWithValue("@Rate", 2.5);
                    command.Parameters.AddWithValue("@Category_ID", CategoryId);
                    command.Parameters.AddWithValue("@Seller_ID", Seller_ID);
                    command.Parameters.AddWithValue("@Sub_Category_ID", SubCategoryId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }








        private DataTable ReadExcelFile(string filePath)
        {
            var dataTable = new DataTable();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new Exception("No worksheet found in the Excel file.");

                // Add columns to DataTable
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    dataTable.Columns.Add(worksheet.Cells[1, col].Text);
                }

                // Add rows to DataTable
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var dataRow = dataTable.NewRow();
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
    }
}
