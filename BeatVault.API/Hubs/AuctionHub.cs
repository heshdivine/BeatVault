using Microsoft.AspNetCore.SignalR;

namespace BeatVault.API.Hubs
{
    // This class handles the "Traffic Control" for real-time messages
    public class AuctionHub : Hub
    {
        // 1. Join a specific Auction Room (Group)
        // When a user opens the page for "Beat #5", they join group "auction-5"
        public async Task JoinAuctionGroup(string auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, auctionId);
        }

        // 2. Leave the group
        public async Task LeaveAuctionGroup(string auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId);
        }
    }
}