using System.Security.Claims; // ADD THIS for ClaimTypes
using BeatVault.API.Data.Repositories;
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
        private readonly IHubContext<AuctionHub> _hubContext;
        private readonly IUserRepository _userRepository;

        public AuctionsController(IAuctionRepository auctionRepo, IHubContext<AuctionHub> hubContext, IUserRepository userRepository)
        {
            _auctionRepository = auctionRepo;
            _hubContext = hubContext;
            _userRepository = userRepository;
        }

        // POST: api/auctions/bid
        [Authorize]
        [HttpPost("bid")]
        public async Task<IActionResult> PlaceBid(int beatId, decimal amount)
        {
            // 1. Get the Auction
            var auction = await _auctionRepository.GetAuctionByBeatIdAsync(beatId);
            if (auction == null) return NotFound("Auction not found");

            // 2. Validate Logic
            if (amount <= auction.CurrentPrice)
                return BadRequest("Bid must be higher than current price");

            if (!auction.IsActive)
                return BadRequest("Auction has ended");

            // 3. IDENTITY FIX: Get the Email specifically
            // We look for the "Email" claim we packed in TokenService
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email)) return Unauthorized("Invalid Token");

            var user = await _userRepository.GetUserByEmailAsync(email);

            // Safety Check: What if user was deleted from DB but still has a token?
            if (user == null) return Unauthorized("User not found");

            var bid = new Bid
            {
                Amount = amount,
                AuctionId = auction.Id,
                UserId = user.Id // Now safe because we checked for null
            };

            auction.CurrentPrice = amount;

            await _auctionRepository.AddBidAsync(bid);
            await _auctionRepository.UpdateAuctionAsync(auction);

            if (await _auctionRepository.SaveChangesAsync())
            {
                // 4. SignalR Broadcast
                await _hubContext.Clients.Group(beatId.ToString())
                    .SendAsync("ReceiveNewBid", new { NewPrice = amount, BidderName = user.Username });

                return Ok("Bid Placed");
            }

            return BadRequest("Failed to place bid");
        }
    }
}