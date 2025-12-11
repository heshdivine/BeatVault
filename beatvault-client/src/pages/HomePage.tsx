import { useEffect, useState } from 'react';
import agent from '../api/agent';
import { Play, ShoppingCart } from 'lucide-react';
import type { Beat } from '../models/beat';

function HomePage() {
    const [beats, setBeats] = useState<Beat[]>([]);

    useEffect(() => {
        // Fetch data when component loads
        agent.Beats.list()
            .then(response => {
                setBeats(response);
            })
            .catch(error => console.error(error));
    }, []);

    return (
        <div className="min-h-screen bg-gray-900 text-white p-10">
            <h1 className="text-4xl font-bold mb-8 text-center text-purple-400">BeatVault Store</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {beats.map(beat => (
                    <div key={beat.id} className="bg-gray-800 rounded-lg p-4 hover:shadow-xl transition shadow-purple-500/20 border border-gray-700">
                        <div className="flex justify-between items-start mb-4">
                            <div>
                                <h2 className="text-xl font-bold">{beat.title}</h2>
                                <p className="text-gray-400 text-sm">Prod. {beat.producerName}</p>
                            </div>
                            <div className="bg-gray-700 px-2 py-1 rounded text-xs">
                                {beat.bpm} BPM
                            </div>
                        </div>

                        <div className="flex items-center gap-4 mt-4">
                            {/* Play Button */}
                            <button className="bg-purple-600 hover:bg-purple-700 text-white p-3 rounded-full transition">
                                <Play size={20} fill="white" />
                            </button>

                            <div className="flex-1">
                                <div className="h-1 bg-gray-600 rounded-full mt-2">
                                    <div className="h-1 bg-purple-500 w-1/3 rounded-full"></div>
                                </div>
                            </div>
                        </div>

                        <div className="mt-6 flex justify-between items-center">
                            <span className="text-2xl font-bold text-green-400">
                                ${beat.leasePrice}
                            </span>
                            <button className="flex items-center gap-2 bg-white text-black px-4 py-2 rounded font-bold hover:bg-gray-200">
                                <ShoppingCart size={18} />
                                Buy Lease
                            </button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default HomePage;