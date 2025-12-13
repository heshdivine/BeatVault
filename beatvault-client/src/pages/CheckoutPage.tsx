import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { loadStripe } from '@stripe/stripe-js';
import { Elements, PaymentElement, useStripe, useElements } from '@stripe/react-stripe-js';
import agent from '../api/agent';

// Replace with your PUBLIC Key (pk_test_...) from Stripe Dashboard
const stripePromise = loadStripe('pk_test_51SdxCrA5y4mytiUmrkFkN140l5WT5A0iKrZPcfGbJc1W1Vb2MhIGMxKufbZ0eRf7p4xnDXH5XBnBejUIpAWxjDVY00tloSVX4Z');

// The Form Component (Needs to be inside Elements provider)
const CheckoutForm = () => {
    const stripe = useStripe();
    const elements = useElements();
    const [message, setMessage] = useState<string | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!stripe || !elements) return;

        setIsProcessing(true);

        // Confirm the payment with Stripe
        const { error } = await stripe.confirmPayment({
            elements,
            confirmParams: {
                // Where to go after success
                return_url: `${window.location.origin}/success`,
            },
        });

        if (error) setMessage(error.message ?? "An error occurred");

        setIsProcessing(false);
    };

    return (
        <form onSubmit={handleSubmit} className="bg-gray-800 p-6 rounded-lg shadow-lg max-w-md w-full">
            <h2 className="text-2xl font-bold mb-6 text-white">Secure Checkout</h2>

            {/* The Actual Stripe UI (Card Number, Expiry, CVC) */}
            <div className="mb-6 bg-gray-700 p-4 rounded border border-gray-600">
                <PaymentElement />
            </div>

            <button
                disabled={isProcessing || !stripe}
                className="w-full bg-green-500 hover:bg-green-600 text-white font-bold py-3 rounded transition disabled:opacity-50"
            >
                {isProcessing ? "Processing..." : "Pay Now"}
            </button>
            {message && <div className="mt-4 text-red-400 text-sm text-center">{message}</div>}
        </form>
    );
};

// The Page Wrapper
export default function CheckoutPage() {
    const { beatId } = useParams();
    const [clientSecret, setClientSecret] = useState('');

    useEffect(() => {
        if (beatId) {
            // Ask Backend for the "Secret" to start transaction
            agent.Payments.createIntent(parseInt(beatId))
                .then(res => setClientSecret(res.clientSecret))
                .catch(err => console.error("Payment setup failed", err));
        }
    }, [beatId]);

    return (
        <div className="min-h-screen bg-gray-900 flex items-center justify-center p-4">
            {clientSecret ? (
                // We wrap the form in the Stripe Context
                <Elements stripe={stripePromise} options={{ clientSecret, appearance: { theme: 'night' } }}>
                    <CheckoutForm />
                </Elements>
            ) : (
                <div className="text-white animate-pulse">Initializing Secure Connection...</div>
            )}
        </div>
    );
}