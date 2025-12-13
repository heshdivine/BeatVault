import { useState } from 'react';
import agent from '../api/agent';
import { UploadCloud, Music, Image as ImageIcon } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function UploadPage() {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    // State for Text Fields
    const [formData, setFormData] = useState({
        title: '',
        bpm: 120,
        key: 'Cm',
        leasePrice: 29.99
    });

    // State for Files (Separate because they are objects, not strings)
    const [audioFile, setAudioFile] = useState<File | null>(null);
    const [coverFile, setCoverFile] = useState<File | null>(null);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!audioFile) {
            alert("Please upload an MP3 file!");
            return;
        }

        setLoading(true);

        try {
            // 1. Create the FormData Package
            // We cannot send JSON. We must pack it into a virtual "Form"
            const data = new FormData();

            // Add Text
            data.append('title', formData.title);
            data.append('bpm', formData.bpm.toString());
            data.append('key', formData.key);
            data.append('leasePrice', formData.leasePrice.toString());

            // Add Files (Must match the Backend Name "audioFile")
            data.append('audioFile', audioFile);

            if (coverFile) {
                data.append('coverImage', coverFile);
            }

            // 2. Send to API
            await agent.Beats.create(data);

            alert('Beat Uploaded Successfully!');
            navigate('/'); // Go back to store
        } catch (error) {
            console.error(error);
            alert('Upload failed. Check console.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gray-900 text-white flex justify-center items-center p-6">
            <form onSubmit={handleSubmit} className="bg-gray-800 p-8 rounded-2xl shadow-2xl border border-gray-700 w-full max-w-lg">
                <h2 className="text-3xl font-bold mb-8 text-center flex items-center justify-center gap-3">
                    <UploadCloud className="text-purple-500" size={32} />
                    Upload New Beat
                </h2>

                {/* Title */}
                <div className="mb-4">
                    <label className="block text-gray-400 text-sm mb-2">Beat Title</label>
                    <input
                        className="w-full bg-gray-700 p-3 rounded border border-gray-600 focus:border-purple-500 outline-none transition"
                        placeholder="e.g. Midnight Fire"
                        value={formData.title}
                        onChange={e => setFormData({ ...formData, title: e.target.value })}
                        required
                    />
                </div>

                {/* Grid for BPM & Key */}
                <div className="grid grid-cols-2 gap-4 mb-4">
                    <div>
                        <label className="block text-gray-400 text-sm mb-2">BPM</label>
                        <input
                            type="number"
                            className="w-full bg-gray-700 p-3 rounded border border-gray-600 focus:border-purple-500 outline-none"
                            value={formData.bpm}
                            onChange={e => setFormData({ ...formData, bpm: parseInt(e.target.value) })}
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-gray-400 text-sm mb-2">Key</label>
                        <input
                            className="w-full bg-gray-700 p-3 rounded border border-gray-600 focus:border-purple-500 outline-none"
                            placeholder="e.g. Cm"
                            value={formData.key}
                            onChange={e => setFormData({ ...formData, key: e.target.value })}
                            required
                        />
                    </div>
                </div>

                {/* Price */}
                <div className="mb-6">
                    <label className="block text-gray-400 text-sm mb-2">Lease Price ($)</label>
                    <input
                        type="number" step="0.01"
                        className="w-full bg-gray-700 p-3 rounded border border-gray-600 focus:border-purple-500 outline-none"
                        value={formData.leasePrice}
                        onChange={e => setFormData({ ...formData, leasePrice: parseFloat(e.target.value) })}
                        required
                    />
                </div>

                {/* FILE UPLOAD: AUDIO */}
                <div className="mb-4">
                    <label className="block text-gray-400 text-sm mb-2">Audio File (MP3)</label>
                    <div className="relative border-2 border-dashed border-gray-600 rounded-lg p-6 hover:border-purple-500 transition group text-center cursor-pointer">
                        <input
                            type="file" accept="audio/*"
                            className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
                            onChange={e => setAudioFile(e.target.files ? e.target.files[0] : null)}
                        />
                        <Music className="mx-auto mb-2 text-gray-500 group-hover:text-purple-400" />
                        <span className="text-sm text-gray-400 group-hover:text-white">
                            {audioFile ? audioFile.name : "Click to Upload Audio"}
                        </span>
                    </div>
                </div>

                {/* FILE UPLOAD: COVER */}
                <div className="mb-8">
                    <label className="block text-gray-400 text-sm mb-2">Cover Art (Optional)</label>
                    <div className="relative border-2 border-dashed border-gray-600 rounded-lg p-6 hover:border-purple-500 transition group text-center cursor-pointer">
                        <input
                            type="file" accept="image/*"
                            className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
                            onChange={e => setCoverFile(e.target.files ? e.target.files[0] : null)}
                        />
                        <ImageIcon className="mx-auto mb-2 text-gray-500 group-hover:text-purple-400" />
                        <span className="text-sm text-gray-400 group-hover:text-white">
                            {coverFile ? coverFile.name : "Click to Upload Image"}
                        </span>
                    </div>
                </div>

                <button
                    disabled={loading}
                    className="w-full bg-purple-600 hover:bg-purple-700 text-white font-bold py-3 rounded transition disabled:bg-purple-900"
                >
                    {loading ? "Uploading..." : "Publish Beat"}
                </button>
            </form>
        </div>
    );
}