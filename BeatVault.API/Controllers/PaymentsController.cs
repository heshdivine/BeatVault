using Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BeatVault.API.Interfaces;

namespace BeatVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IBeatRepository _beatRepository;
        private readonly IConfiguration _config;

        public PaymentsController(IBeatRepository beatRepository, IConfiguration config)
        {
            _beatRepository = beatRepository;
            _config = config;
        }

        [Authorize]
        [HttpPost("create-payment-intent")]
        public async Task<ActionResult<string>> CreatePaymentIntent(int beatId)
        {
            // 1. Get the Product from DB (Secure Price Check)
            var beat = await _beatRepository.GetBeatByIdAsync(beatId);

            if (beat == null) return NotFound();
            if (beat.LeasePrice == null) return BadRequest("This beat is not for sale");

            // 2. Initialize Stripe
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            // 3. Create the Intent
            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(new PaymentIntentCreateOptions
            {
                // Stripe uses "cents" (e.g., $20.00 = 2000 cents)
                Amount = (long)(beat.LeasePrice.Value * 100),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                // Metadata helps us track what they bought later
                Metadata = new Dictionary<string, string>
                {
                    { "BeatId", beat.Id.ToString() },
                    { "BeatTitle", beat.Title }
                }
            });

            // 4. Return the "Client Secret" (The key the Frontend needs to finish the payment)
            return Ok(new { clientSecret = intent.ClientSecret });
        }
    }
}