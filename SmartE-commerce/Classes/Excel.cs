namespace SmartE_commerce.Classes
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using Microsoft.Data.SqlClient;
    using OfficeOpenXml;

    public class Excel
    {
        public Excel() {
            string excelFilePath = @"D:\DATA\data1.xlsx";

            // Connection string to your SQL database
            string connectionString = "server=.;database=Smart_EcommerceV2;integrated security =true; trust server certificate = true ";

            // Load the Excel file
            using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming first worksheet
                int rowCount = worksheet.Dimension.Rows;

                // Open SQL connection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    for (int row = 2; row <= rowCount; row++) // Start from row 2 to skip headers
                    {
                        string col1 = worksheet.Cells[row, 1].Text; // Column 1 data
                        string col2 = worksheet.Cells[row, 2].Text; // Column 2 data
                                                                    // Add more columns as needed

                        // SQL query
                        string query = "INSERT INTO Category (Category_Name, Image) VALUES (@col1, @col2)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@col1", col1);
                            command.Parameters.AddWithValue("@col2", col2);

                            // Execute query
                            command.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
            }

            Console.WriteLine("Data inserted successfully!");
        }
    }
}
