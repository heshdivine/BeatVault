import { useState } from 'react';
import agent from '../api/agent';
import { useNavigate, Link } from 'react-router-dom';
import { UserPlus, Music, Palette } from 'lucide-react';
import { useUser } from '../context/UserContext';

export default function SignupPage() {
    const navigate = useNavigate();
    const { login } = useUser();
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        role: 'Producer' // Default to Producer
    });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const user = await agent.Account.register(formData);
            login(user); // Use context to log in
            navigate('/'); // Redirect to home
        } catch (err: any) {
            setError(err.response?.data || 'Registration failed. Please try again.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-900 text-white p-6">
            <form onSubmit={handleSubmit} className="bg-gray-800 p-8 rounded-2xl shadow-2xl w-full max-w-md border border-gray-700">
                <div className="text-center mb-8">
                    <div className="flex items-center justify-center gap-2 mb-2">
                        <UserPlus className="text-purple-400" size={32} />
                        <h2 className="text-3xl font-bold text-purple-400">Join BeatVault</h2>
                    </div>
                    <p className="text-gray-400 text-sm">Create your account and start your journey</p>
                </div>

                {error && (
                    <div className="bg-red-500/20 text-red-400 p-3 mb-4 rounded border border-red-500/50">
                        {error}
                    </div>
                )}

                {/* Role Selection */}
                <div className="mb-6">
                    <label className="block mb-3 text-sm text-gray-400 font-medium">I am a...</label>
                    <div className="grid grid-cols-2 gap-3">
                        <button
                            type="button"
                            onClick={() => setFormData({ ...formData, role: 'Producer' })}
                            className={`p-4 rounded-lg border-2 transition-all ${formData.role === 'Producer'
                                    ? 'border-purple-500 bg-purple-500/20 shadow-lg shadow-purple-500/20'
                                    : 'border-gray-600 bg-gray-700/50 hover:border-gray-500'
                                }`}
                        >
                            <Music className={`mx-auto mb-2 ${formData.role === 'Producer' ? 'text-purple-400' : 'text-gray-400'}`} size={24} />
                            <div className={`font-bold ${formData.role === 'Producer' ? 'text-purple-300' : 'text-gray-300'}`}>
                                Producer
                            </div>
                            <div className="text-xs text-gray-400 mt-1">Upload & sell beats</div>
                        </button>

                        <button
                            type="button"
                            onClick={() => setFormData({ ...formData, role: 'Artist' })}
                            className={`p-4 rounded-lg border-2 transition-all ${formData.role === 'Artist'
                                    ? 'border-purple-500 bg-purple-500/20 shadow-lg shadow-purple-500/20'
                                    : 'border-gray-600 bg-gray-700/50 hover:border-gray-500'
                                }`}
                        >
                            <Palette className={`mx-auto mb-2 ${formData.role === 'Artist' ? 'text-purple-400' : 'text-gray-400'}`} size={24} />
                            <div className={`font-bold ${formData.role === 'Artist' ? 'text-purple-300' : 'text-gray-300'}`}>
                                Artist
                            </div>
                            <div className="text-xs text-gray-400 mt-1">Browse & buy beats</div>
                        </button>
                    </div>
                </div>

                {/* Username */}
                <div className="mb-4">
                    <label className="block mb-2 text-sm text-gray-400">Username</label>
                    <input
                        type="text"
                        className="w-full p-3 rounded-lg bg-gray-700 border border-gray-600 focus:border-purple-500 outline-none transition"
                        placeholder="Enter your username"
                        value={formData.username}
                        onChange={e => setFormData({ ...formData, username: e.target.value })}
                        required
                    />
                </div>

                {/* Email */}
                <div className="mb-4">
                    <label className="block mb-2 text-sm text-gray-400">Email</label>
                    <input
                        type="email"
                        className="w-full p-3 rounded-lg bg-gray-700 border border-gray-600 focus:border-purple-500 outline-none transition"
                        placeholder="your@email.com"
                        value={formData.email}
                        onChange={e => setFormData({ ...formData, email: e.target.value })}
                        required
                    />
                </div>

                {/* Password */}
                <div className="mb-6">
                    <label className="block mb-2 text-sm text-gray-400">Password</label>
                    <input
                        type="password"
                        className="w-full p-3 rounded-lg bg-gray-700 border border-gray-600 focus:border-purple-500 outline-none transition"
                        placeholder="Minimum 6 characters"
                        value={formData.password}
                        onChange={e => setFormData({ ...formData, password: e.target.value })}
                        required
                        minLength={6}
                    />
                </div>

                <button
                    disabled={loading}
                    className="w-full bg-purple-600 hover:bg-purple-700 text-white font-bold py-3 rounded-lg transition disabled:bg-purple-900 disabled:cursor-not-allowed"
                >
                    {loading ? 'Creating Account...' : 'Sign Up'}
                </button>

                <div className="mt-6 text-center text-sm text-gray-400">
                    Already have an account?{' '}
                    <Link to="/login" className="text-purple-400 hover:text-purple-300 font-medium">
                        Log in
                    </Link>
                </div>
            </form>
        </div>
    );
}
