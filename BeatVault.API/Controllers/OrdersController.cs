using Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BeatVault.API.Interfaces;
using BeatVault.API.Entities;

namespace BeatVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IOrderRepository _orderRepo; 
        private readonly IUserRepository _userRepo;

        public OrdersController(IConfiguration config, IOrderRepository orderRepo, IUserRepository userRepo)
        {
            _config = config;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
        }
        [Authorize]
        [HttpPost("confirm-purchase")]
        public async Task<IActionResult> ConfirmPurchase([FromQuery] string paymentIntentId)
        {
            // 1. Setup Stripe & Verify
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            var service = new PaymentIntentService();
            var intent = await service.GetAsync(paymentIntentId);

            if (intent.Status != "succeeded")
                return BadRequest("Payment Verification Failed.");

            // 2. Check if Order already exists (Prevent duplicate saves if user refreshes page)
            // (Ideally, you'd check DB here, but for now we proceed)

            // 3. Get User ID from Claims (The person currently logged in)
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var user = await _userRepo.GetUserByEmailAsync(email);

            if (user == null) return Unauthorized("User not found");

            // 4. Create the Order Object
            var order = new Order
            {
                UserId = user.Id,
                BeatId = int.Parse(intent.Metadata["BeatId"]), // Read from Stripe Metadata
                PricePaid = intent.Amount / 100m, // Convert cents back to dollars
                StripePaymentIntentId = intent.Id,
                OrderDate = DateTime.UtcNow
            };

            // 5. SAVE TO DB
            await _orderRepo.CreateOrderAsync(order);

            return Ok(new { message = "Order Confirmed and Saved!" });
        }
    }
}