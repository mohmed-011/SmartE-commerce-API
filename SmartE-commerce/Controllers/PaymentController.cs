using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartE_commerce.Classes;
using SmartE_commerce.Data;
using SmartE_commerce.Services;
using System.Data;
using System.Text.Json;

namespace SmartE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : ControllerBase
    {
        private readonly PaymobService _paymobService;
        private readonly string _connectionString;

        public PaymentController(CartService cartService, IConfiguration configuration)
        {
            _paymobService = new(cartService);
            _connectionString = configuration.GetConnectionString("MyDatabase");

        }

        [HttpPost("start")]
        public async Task<IActionResult> StartPayment([FromBody] BillingData billingData , int integrationId , int userId,int addressID)
        {
            try
            {
               
            var token = await _paymobService.GetAuthToken();
            var orderId = await _paymobService.CreateOrder(token , userId);
            var paymentKey = await _paymobService.GetPaymentKey(token, orderId , billingData,  integrationId, userId , addressID);
            var url = _paymobService.GetPaymentUrl(paymentKey);
            return Ok(new
            {
                token,
                orderId,
                paymentKey,
                paymentUrl = url
            });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"🔥 Exception: {ex.Message}");
            }
        }


        [HttpGet("payment-redirect")]
        public async Task<IActionResult> PaymentRedirect([FromQuery] int userId , [FromQuery] int addressID)
        {
            var query = HttpContext.Request.Query;

            string successStr = query["success"]!;
            string txRef = query["txn_response_code"]!;
            string orderIdStr = query["order"]!;
            string amountCentsStr = query["amount_cents"]!;
            string txnId = query["id"]!;

            bool isSuccess = successStr == "true";

            var payment = new Payment
            {
                TransactionId = txnId,
                OrderId = int.TryParse(orderIdStr, out int oid) ? oid : 0,
                Amount = decimal.TryParse(amountCentsStr, out var a) ? a / 100 : 0,
                PaymentStatus = isSuccess ? "Success" : "Failed",
                PaymentMethod = "Redirect",
                PaymentDate = DateTime.UtcNow,
                UserId = userId,
                AddressID = addressID, // أو جيبه من order table لو عندك
                // أو جيبه من order table لو عندك
                RawResponse = JsonSerializer.Serialize(query.ToDictionary(q => q.Key, q => q.Value.ToString()))
            };


            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("AddPayment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TransactionId", payment.TransactionId);
                        command.Parameters.AddWithValue("@OrderId", payment.OrderId);
                        command.Parameters.AddWithValue("@Amount", payment.Amount);
                        command.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
                        command.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
                        command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                        command.Parameters.AddWithValue("@UserId", payment.UserId);
                        command.Parameters.AddWithValue("@AddressId", payment.AddressID);

                        command.Parameters.AddWithValue("@RawResponse", payment.RawResponse);




                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok($"Order with id : {payment.OrderId} Saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

    }
}
