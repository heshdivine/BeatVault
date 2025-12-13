using Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BeatVault.API.Interfaces;

namespace BeatVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize]
        [HttpPost("confirm-purchase")]
        public async Task<IActionResult> ConfirmPurchase([FromQuery] string paymentIntentId)
        {
            // 1. Setup Stripe
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            var service = new PaymentIntentService();

            // 2. Ask Stripe: "Is this transaction real?"
            var intent = await service.GetAsync(paymentIntentId);

            // 3. Verify Status
            if (intent.Status != "succeeded")
                return BadRequest("Payment Verification Failed: Status is " + intent.Status);

            // 4. Extract Data we hid in Metadata earlier
            var beatId = intent.Metadata["BeatId"];
            var beatTitle = intent.Metadata["BeatTitle"];

            // 5. TODO: Save to 'Orders' table in Database
            // _orderRepo.CreateOrder(userId, beatId, price);

            return Ok(new { message = $"Success! You purchased {beatTitle}" });
        }
    }
}