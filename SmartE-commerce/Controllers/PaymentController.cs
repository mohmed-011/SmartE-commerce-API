using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartE_commerce.Classes;
using SmartE_commerce.Services;
using System.Text.Json;

namespace SmartE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController(CartService cartService) : ControllerBase
    {
        private readonly PaymobService _paymobService = new(cartService);

        [HttpPost("start")]
        public async Task<IActionResult> StartPayment([FromBody] BillingData billingData , int integrationId , int userId)
        {
            try
            {
               
            var token = await _paymobService.GetAuthToken();
            var orderId = await _paymobService.CreateOrder(token , userId);
            var paymentKey = await _paymobService.GetPaymentKey(token, orderId , billingData,  integrationId, userId);
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
        public async Task<IActionResult> PaymentRedirect([FromQuery] int userId)
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
                UserId = userId, // أو جيبه من order table لو عندك
                RawResponse = JsonSerializer.Serialize(query.ToDictionary(q => q.Key, q => q.Value.ToString()))
            };

           

            return Ok(payment);
        }

    }
}
