import { useEffect, useState } from 'react';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { useParams } from 'react-router-dom';
import { Gavel, TrendingUp } from 'lucide-react';
import agent from '../api/agent';

export default function AuctionPage() {
    const { id } = useParams(); // Get beat ID from URL
    const [hubConnection, setHubConnection] = useState<HubConnection | null>(null);

    // State for the Auction Data
    const [currentPrice, setCurrentPrice] = useState(0);
    const [bidAmount, setBidAmount] = useState(0);
    const [messages, setMessages] = useState<string[]>([]);

    useEffect(() => {
        if (id) {
            agent.Auctions.details(id)
                .then(auction => {
                    setCurrentPrice(auction.currentPrice);
                    // Optional: set initial message for highest bidder
                    if (auction.highestBidder !== "No Bids Yet") {
                        setMessages([`Current leader: ${auction.highestBidder}`]);
                    }
                })
                .catch(error => console.error("Could not load auction:", error));
        }

        // 2. Build SignalR Connection
        const connection = new HubConnectionBuilder()
            .withUrl('https://localhost:7144/hubs/auction') // Backend Hub URL
            .withAutomaticReconnect()
            .build();

        // 3. Start Connection
        connection.start()
            .then(() => {
                console.log('Connected to Hub');
                // Join the specific room for this beat
                connection.invoke('JoinAuctionGroup', id);
                setHubConnection(connection);
            })
            .catch(err => console.error('Connection failed: ', err));

        // 4. LISTEN FOR UPDATES (The Magic)
        connection.on('ReceiveNewBid', (data: any) => {
            setCurrentPrice(data.newPrice);
            setMessages(prev => [`New highest bid: $${data.newPrice} by User #${data.bidderName}`, ...prev]);
        });

        // Cleanup when leaving page
        return () => {
            connection.stop();
        };
    }, [id]);

    const placeBid = async () => {
        if (!hubConnection) return;
        try {
            // Call REST API to securely process bid
            // Note: We cast id to number because URL params are strings
            await agent.Auctions.bid(id!, bidAmount).then(() => {
                setBidAmount(0); // Clear input
            });
        } catch (error) {
            alert('Bid failed! Make sure you are logged in and amount is higher.');
        }
    };

    return (
        <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center p-10">
            <div className="bg-gray-800 p-8 rounded-2xl shadow-2xl border border-purple-500/30 w-full max-w-2xl">

                <div className="flex justify-between items-center mb-8">
                    <h1 className="text-3xl font-bold flex items-center gap-3">
                        <Gavel className="text-purple-500" /> Exclusive Rights Auction
                    </h1>
                    <div className="bg-red-500/20 text-red-400 px-4 py-1 rounded-full text-sm font-bold animate-pulse">
                        LIVE
                    </div>
                </div>

                <div className="text-center mb-10">
                    <p className="text-gray-400 mb-2">Current Highest Bid</p>
                    <div className="text-6xl font-black text-green-400 flex justify-center items-center gap-2">
                        ${currentPrice.toFixed(2)} <TrendingUp size={40} />
                    </div>
                </div>

                {/* Bidding Controls */}
                <div className="flex gap-4 mb-8">
                    <input
                        type="number"
                        className="flex-1 bg-gray-700 rounded-lg p-4 text-xl border border-gray-600 focus:border-purple-500 outline-none"
                        placeholder="Enter amount..."
                        value={bidAmount}
                        onChange={e => setBidAmount(parseFloat(e.target.value))}
                    />
                    <button
                        onClick={placeBid}
                        className="bg-purple-600 hover:bg-purple-700 text-white text-xl font-bold px-8 rounded-lg transition"
                    >
                        Place Bid
                    </button>
                </div>

                {/* Live Ticker */}
                <div className="bg-black/40 rounded-lg p-4 h-40 overflow-y-auto">
                    <h3 className="text-gray-400 text-sm mb-2 uppercase tracking-wider">Live Activity</h3>
                    {messages.map((msg, i) => (
                        <div key={i} className="text-sm text-gray-300 py-1 border-b border-gray-800 last:border-0">
                            {msg}
                        </div>
                    ))}
                    {messages.length === 0 && <p className="text-gray-600 italic">Waiting for bids...</p>}
                </div>
            </div>
        </div>
    );
}