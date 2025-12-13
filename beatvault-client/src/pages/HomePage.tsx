import { useEffect, useState } from 'react';
import agent from '../api/agent';
import type { Beat } from '../models/beat';
import { Play, Pause, ShoppingCart, Gavel } from 'lucide-react'; // Added Pause icon
import { useNavigate } from 'react-router-dom';
import MusicPlayer from '../components/MusicPlayer';

export default function HomePage() {
    const navigate = useNavigate();
    const [beats, setBeats] = useState<Beat[]>([]);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [volume, setVolume] = useState(0.7); // Default 70% volume

    // AUDIO STATE - Declared before useEffect that depends on it
    const [playingId, setPlayingId] = useState<number | null>(null);
    const [currentAudio, setCurrentAudio] = useState<HTMLAudioElement | null>(null);

    // Use an effect to update progress
    useEffect(() => {
        if (!currentAudio) return;

        const updateProgress = () => {
            setCurrentTime(currentAudio.currentTime);
            setDuration(currentAudio.duration);
        };

        currentAudio.addEventListener('timeupdate', updateProgress);
        return () => currentAudio.removeEventListener('timeupdate', updateProgress);
    }, [currentAudio]);

    const handleSeek = (val: number) => {
        if (currentAudio) {
            currentAudio.currentTime = val;
            setCurrentTime(val);
        }
    };

    const handleVolumeChange = (val: number) => {
        const newVolume = val / 100; // Convert 0-100 to 0-1
        setVolume(newVolume);
        if (currentAudio) {
            currentAudio.volume = newVolume;
        }
    };

    useEffect(() => {
        agent.Beats.list()
            .then(response => setBeats(response))
            .catch(error => console.error(error));

        // Cleanup: Stop music if user leaves the page
        return () => {
            if (currentAudio) {
                currentAudio.pause();
                currentAudio.src = "";
            }
        };
    }, []); // Empty dependency array = run once on mount

    // THE SMART PLAYER LOGIC
    const togglePlay = (beat: Beat) => {
        // Scenario 1: User clicks the song that is ALREADY playing
        if (playingId === beat.id) {
            if (currentAudio?.paused) {
                currentAudio.play();
                setPlayingId(beat.id); // Force re-render to show Pause icon
            } else {
                currentAudio?.pause();
                setPlayingId(null); // Show Play icon
            }
            return;
        }

        // Scenario 2: User clicks a NEW song
        // A. Stop the old one (if exists)
        if (currentAudio) {
            currentAudio.pause();
            currentAudio.currentTime = 0; // Rewind
        }

        // B. Create new audio
        const newAudio = new Audio(beat.audioUrl);
        newAudio.volume = volume; // Use current volume state

        // C. Handle when song finishes naturally
        newAudio.onended = () => setPlayingId(null);

        // D. Play it
        newAudio.play().catch(e => console.error("Playback failed:", e));

        setCurrentAudio(newAudio);
        setPlayingId(beat.id);
    };

    return (
        <div className="min-h-screen bg-gray-900 text-white p-10 pt-24"> {/* Added padding top for nav */}
            <h1 className="text-4xl font-bold mb-8 text-center text-purple-400">BeatVault Store</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {beats.map(beat => (
                    <div key={beat.id} className={`bg-gray-800 rounded-lg p-4 hover:shadow-xl transition border ${playingId === beat.id ? 'border-purple-500 shadow-purple-500/20' : 'border-gray-700'}`}>
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
                            {/* PLAY/PAUSE BUTTON */}
                            <button
                                onClick={() => togglePlay(beat)}
                                className="bg-purple-600 hover:bg-purple-700 text-white p-3 rounded-full transition shadow-lg hover:scale-110"
                            >
                                {/* Dynamically show Play or Pause based on state */}
                                {playingId === beat.id ? (
                                    <Pause size={20} fill="white" />
                                ) : (
                                    <Play size={20} fill="white" />
                                )}
                            </button>

                            {/* Visual Waveform Bar (Animation) */}
                            <div className="flex-1 h-8 flex items-center gap-1">
                                {playingId === beat.id ? (
                                    // Fake waveform animation when playing
                                    [...Array(10)].map((_, i) => (
                                        <div key={i} className="w-1 bg-purple-500 animate-pulse" style={{ height: `${Math.random() * 100}%`, animationDuration: `${0.5 + Math.random()}s` }}></div>
                                    ))
                                ) : (
                                    // Static line when paused
                                    <div className="h-1 bg-gray-600 w-full rounded-full"></div>
                                )}
                            </div>
                        </div>

                        <div className="mt-6 flex justify-between items-center">
                            {/* LEASE BUTTON */}
                            {beat.leasePrice && beat.auctionId == null && (
                                <>
                                    <span className="text-2xl font-bold text-green-400">${beat.leasePrice}</span>
                                    <button className="flex items-center gap-2 bg-white text-black px-4 py-2 rounded font-bold hover:bg-gray-200 transition">
                                        <ShoppingCart size={18} /> Buy
                                    </button>
                                </>
                            )}

                            {/* AUCTION BUTTON */}
                            {beat.auctionId && (
                                <button
                                    onClick={() => navigate(`/auction/${beat.auctionId}`)}
                                    className="flex items-center gap-2 bg-purple-600 text-white px-4 py-2 rounded font-bold hover:bg-purple-700 w-full justify-center transition"
                                >
                                    <Gavel size={18} /> Bid Live
                                </button>
                            )}
                        </div>
                    </div>
                ))}
            </div>
            {playingId && (
                <MusicPlayer
                    beat={beats.find(b => b.id === playingId)}
                    isPlaying={!currentAudio?.paused}
                    onToggle={() => togglePlay(beats.find(b => b.id === playingId)!)}
                    currentTime={currentTime}
                    duration={duration}
                    onSeek={handleSeek}
                    progress={(currentTime / duration) * 100}
                    volume={volume * 100}
                    onVolumeChange={handleVolumeChange}
                />
            )}
        </div>

    );
}