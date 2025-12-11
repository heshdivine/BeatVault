import { useState } from 'react';
import agent from '../api/agent';
import { useNavigate } from 'react-router-dom';

export default function LoginPage() {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({ email: '', password: '' });
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const user = await agent.Account.login(formData);
            // STORE THE TOKEN! This is key.
            localStorage.setItem('jwt', user.token);
            alert('Welcome back, ' + user.username);
            navigate('/'); // Go back home
        } catch (err) {
            setError('Invalid login attempt');
            console.log(err);
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-900 text-white">
            <form onSubmit={handleSubmit} className="bg-gray-800 p-8 rounded-lg shadow-lg w-96 border border-gray-700">
                <h2 className="text-3xl font-bold mb-6 text-center text-purple-400">Producer Login</h2>

                {error && <div className="bg-red-500/20 text-red-400 p-3 mb-4 rounded">{error}</div>}

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

                <button className="w-full bg-purple-600 hover:bg-purple-700 text-white font-bold py-2 rounded transition">
                    Sign In
                </button>
            </form>
        </div>
    );
}