import { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import agent from '../api/agent';
import { CheckCircle } from 'lucide-react';

export default function SuccessPage() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [status, setStatus] = useState('Verifying Payment...');

    // Stripe puts 'payment_intent' in the URL query params automatically
    const paymentIntentId = searchParams.get('payment_intent');
    const redirectStatus = searchParams.get('redirect_status');

    useEffect(() => {
        if (!paymentIntentId || redirectStatus !== 'succeeded') {
            setStatus('Payment processing failed or was cancelled.');
            return;
        }

        // Call our backend to confirm it's real
        agent.Payments.confirm(paymentIntentId)
            .then(() => {
                setStatus('Payment Verified! Your license has been generated.');
                // Optional: Redirect to home after 4 seconds
                setTimeout(() => navigate('/'), 4000);
            })
            .catch(err => {
                console.error(err);
                setStatus('Payment succeeded, but server verification failed. Contact support.');
            });
    }, [paymentIntentId, redirectStatus, navigate]);

    return (
        <div className="min-h-screen bg-gray-900 flex items-center justify-center text-white">
            <div className="bg-gray-800 p-10 rounded-2xl shadow-2xl text-center max-w-md border border-green-500/30">
                <CheckCircle className="mx-auto text-green-500 w-24 h-24 mb-6 animate-bounce" />
                <h2 className="text-3xl font-bold mb-4 text-white">Thank You!</h2>
                <p className="text-gray-300 text-lg mb-6">{status}</p>
                <div className="w-full bg-gray-700 h-1 mt-4 rounded-full overflow-hidden">
                    <div className="h-full bg-green-500 animate-[pulse_2s_infinite]"></div>
                </div>
                <p className="text-xs text-gray-500 mt-4">Redirecting you to the studio...</p>
            </div>
        </div>
    );
}