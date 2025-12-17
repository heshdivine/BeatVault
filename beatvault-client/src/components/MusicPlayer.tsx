import { Play, Pause, SkipBack, SkipForward, Volume2 } from 'lucide-react';

interface Props {
    beat: any;
    isPlaying: boolean;
    onToggle: () => void;
    progress: number;
    onSeek: (value: number) => void;
    duration: number;
    currentTime: number;
    volume: number;
    onVolumeChange: (value: number) => void;
}

export default function MusicPlayer({ beat, isPlaying, onToggle, onSeek, duration, currentTime, volume, onVolumeChange }: Props) {

    // Helper to format seconds into 0:00
    const formatTime = (time: number) => {
        const mins = Math.floor(time / 60);
        const secs = Math.floor(time % 60);
        return `${mins}:${secs < 10 ? '0' : ''}${secs}`;
    };

    return (
        <div className="fixed bottom-0 left-0 right-0 bg-gray-900/95 backdrop-blur-lg border-t border-purple-500/30 p-4 z-50 transition-all transform animate-slide-up">
            <div className="max-w-7xl mx-auto flex items-center gap-6">

                {/* 1. Info Section */}
                <div className="flex items-center gap-4 w-1/4">
                    <img src={beat.coverImageUrl || 'https://via.placeholder.com/50'} className="w-12 h-12 rounded shadow-lg" alt="" />
                    <div className="hidden md:block">
                        <h4 className="font-bold text-sm truncate">{beat.title}</h4>
                        <p className="text-xs text-gray-400">{beat.producerName}</p>
                    </div>
                </div>

                {/* 2. Controls & Seek Section */}
                <div className="flex-1 flex flex-col items-center">
                    <div className="flex items-center gap-6 mb-2">
                        <SkipBack size={20} className="text-gray-400 cursor-pointer hover:text-white" />
                        <button onClick={onToggle} className="bg-white text-black p-2 rounded-full hover:scale-110 transition">
                            {isPlaying ? <Pause size={24} fill="black" /> : <Play size={24} fill="black" />}
                        </button>
                        <SkipForward size={20} className="text-gray-400 cursor-pointer hover:text-white" />
                    </div>

                    <div className="w-full flex items-center gap-3">
                        <span className="text-[10px] text-gray-400 w-8 text-right">{formatTime(currentTime)}</span>
                        <input
                            type="range"
                            min="0"
                            max={duration || 0}
                            value={currentTime}
                            onChange={(e) => onSeek(parseFloat(e.target.value))}
                            className="flex-1 h-1 bg-gray-700 accent-purple-500 cursor-pointer appearance-none rounded-full"
                        />
                        <span className="text-[10px] text-gray-400 w-8">{formatTime(duration)}</span>
                    </div>
                </div>

                {/* 3. Volume Section */}
                <div className="hidden md:flex items-center justify-end gap-2 w-1/4">
                    <Volume2 size={18} className="text-gray-400" />
                    <input
                        type="range"
                        min="0"
                        max="100"
                        value={volume}
                        onChange={(e) => onVolumeChange(parseFloat(e.target.value))}
                        className="w-24 h-1 bg-gray-700 accent-purple-500 cursor-pointer appearance-none rounded-full"
                    />
                </div>
            </div>
        </div>
    );
}