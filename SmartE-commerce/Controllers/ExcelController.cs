using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SmartE_commerce.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly string _connectionString = "server=.;database=Smart_EcommerceV3;integrated security =true; trust server certificate = true ";

        [HttpPost("import")]
        public async Task<IActionResult> ImportExcel([FromForm] IFormFile file)
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
                await InsertDataIntoDatabase(dataTable);

                return Ok("Data imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

        private async Task InsertDataIntoDatabase(DataTable dataTable)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (DataRow row in dataTable.Rows)
                {
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
                    command.Parameters.AddWithValue("@Item_ID", row["Item_ID"] != DBNull.Value ? Convert.ToInt32(row["Item_ID"]) : 0);
                    command.Parameters.AddWithValue("@Image_Cover", row["Image_Cover"] != DBNull.Value && !string.IsNullOrEmpty(row["Image_Cover"].ToString()) ? row["Image_Cover"].ToString() : "Image_Cover.jpg");
                    command.Parameters.AddWithValue("@Item_Name", row["Item_Name"] != DBNull.Value ? row["Item_Name"].ToString() : string.Empty);
                    command.Parameters.AddWithValue("@Description", row["Description"] != DBNull.Value ? row["Description"].ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0);
                    command.Parameters.AddWithValue("@Price_in", row["Price_in"] != DBNull.Value ? Convert.ToDecimal(row["Price_in"]) : 0.0m);
                    command.Parameters.AddWithValue("@Price_out", row["Price_out"] != DBNull.Value ? Convert.ToDecimal(row["Price_out"]) : 0.0m);
                    command.Parameters.AddWithValue("@Discount", row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : DBNull.Value);
                    command.Parameters.AddWithValue("@Rate", row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : DBNull.Value);
                    command.Parameters.AddWithValue("@Category_ID", row["Category_ID"] != DBNull.Value ? Convert.ToInt32(row["Category_ID"]) : 0);
                    command.Parameters.AddWithValue("@Seller_ID", row["Seller_ID"] != DBNull.Value ? Convert.ToInt32(row["Seller_ID"]) : 0);
                    command.Parameters.AddWithValue("@Sub_Category_ID", row["Sub_Category_ID"] != DBNull.Value ? Convert.ToInt32(row["Sub_Category_ID"]) : 0);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
