import { useState } from 'react';
import agent from '../api/agent';
import { useNavigate, Link } from 'react-router-dom';
import { useUser } from '../context/UserContext';
import { LogIn } from 'lucide-react';

export default function LoginPage() {
    const navigate = useNavigate();
    const { login } = useUser();
    const [formData, setFormData] = useState({ email: '', password: '' });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const user = await agent.Account.login(formData);
            login(user); // Use context to log in
            navigate('/'); // Go back home
        } catch (err: any) {
            setError(err.response?.data || 'Invalid email or password');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-900 text-white">
            <form onSubmit={handleSubmit} className="bg-gray-800 p-8 rounded-2xl shadow-2xl w-96 border border-gray-700">
                <div className="text-center mb-8">
                    <div className="flex items-center justify-center gap-2 mb-2">
                        <LogIn className="text-purple-400" size={28} />
                        <h2 className="text-3xl font-bold text-purple-400">Welcome Back</h2>
                    </div>
                    <p className="text-gray-400 text-sm">Sign in to your account</p>
                </div>

                {error && (
                    <div className="bg-red-500/20 text-red-400 p-3 mb-4 rounded border border-red-500/50">
                        {error}
                    </div>
                )}

                <div className="mb-4">
                    <label className="block mb-2 text-sm text-gray-400">Email</label>
                    <input
                        type="email"
                        className="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-purple-500 outline-none"
                        value={formData.email}
                        onChange={e => setFormData({ ...formData, email: e.target.value })}
                    />
                </div>

                <div className="mb-6">
                    <label className="block mb-2 text-sm text-gray-400">Password</label>
                    <input
                        type="password"
                        className="w-full p-2 rounded bg-gray-700 border border-gray-600 focus:border-purple-500 outline-none"
                        value={formData.password}
                        onChange={e => setFormData({ ...formData, password: e.target.value })}
                    />
                </div>

                <button
                    disabled={loading}
                    className="w-full bg-purple-600 hover:bg-purple-700 text-white font-bold py-3 rounded-lg transition disabled:bg-purple-900 disabled:cursor-not-allowed"
                >
                    {loading ? 'Signing In...' : 'Sign In'}
                </button>

                <div className="mt-6 text-center text-sm text-gray-400">
                    Don't have an account?{' '}
                    <Link to="/signup" className="text-purple-400 hover:text-purple-300 font-medium">
                        Sign up
                    </Link>
                </div>
            </form>
        </div>
    );
}