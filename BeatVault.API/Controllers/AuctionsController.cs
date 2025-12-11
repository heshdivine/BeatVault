using BeatVault.API.DTOs;
using BeatVault.API.Entities;
using BeatVault.API.Hubs;
using BeatVault.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BeatVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IHubContext<AuctionHub> _hubContext; // Inject SignalR Hub

        public AuctionsController(IAuctionRepository auctionRepo, IHubContext<AuctionHub> hubContext)
        {
            _auctionRepository = auctionRepo;
            _hubContext = hubContext;
        }

        // POST: api/auctions/bid
        [Authorize] // Must be logged in to bid
        [HttpPost("bid")]
        public async Task<IActionResult> PlaceBid(int beatId, decimal amount)
        {
            // 1. Get the Auction
            var auction = await _auctionRepository.GetAuctionByBeatIdAsync(beatId);
            if (auction == null) return NotFound("Auction not found");

            // 2. Validate Logic (Interview Win: Business Rules)
            if (amount <= auction.CurrentPrice)
                return BadRequest("Bid must be higher than current price");

            if (!auction.IsActive)
                return BadRequest("Auction has ended");

            // 3. Create and Save Bid
            // (In real app, get UserId from Token. For now, we assume user ID 1 for testing)
            var bid = new Bid
            {
                Amount = amount,
                AuctionId = auction.Id,
                UserId = 1 // Placeholder: Replace with User.Identity logic later
            };

            auction.CurrentPrice = amount; // Update the price

            await _auctionRepository.AddBidAsync(bid);
            await _auctionRepository.UpdateAuctionAsync(auction);

            if (await _auctionRepository.SaveChangesAsync())
            {
                // 4. THE MAGIC: SignalR Broadcast
                // Notify ONLY the people looking at this specific auction
                await _hubContext.Clients.Group(beatId.ToString())
                    .SendAsync("ReceiveNewBid", new { NewPrice = amount, BidderId = 1 });

                return Ok("Bid Placed");
            }

            return BadRequest("Failed to place bid");
        }
    }
}