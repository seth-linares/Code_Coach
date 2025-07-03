"use client"

import { useEffect, useState, useRef } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import axios from 'axios';

export default function ConfirmEmailClient() {
    const searchParamsObj = useSearchParams();
    const router = useRouter();
    const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
    const [message, setMessage] = useState('');
    const hasAttemptedConfirmationRef = useRef(false);

    useEffect(() => {
        const handleConfirmEmail = async () => {
            if (!searchParamsObj || hasAttemptedConfirmationRef.current) {
                return;
            }

            hasAttemptedConfirmationRef.current = true;

            const userId = searchParamsObj.get('userId');
            const encodedToken = searchParamsObj.get('token');

            console.log(`Encoded token: ${encodedToken}, UserId: ${userId}`);

            if (userId && encodedToken) {
                try {
                    // Decode the token
                    const token = decodeURIComponent(encodedToken);
                    console.log(`URI Token: ${token}`);

                    const response = await axios.post('https://www.codecoachapp.com/api/Users/ConfirmEmail', { userId, token });
                    setStatus('success');
                    setMessage(response.data.message);
                    setTimeout(() => router.push('/login'), 3000);
                } catch (error) {
                    console.error('Error confirming email:', error);
                    setStatus('error');
                    setMessage('Failed to confirm email. Please try again.');
                }
            } else {
                setStatus('error');
                setMessage('Invalid confirmation link. Please check your email and try again.');
            }
        };

        handleConfirmEmail();
    }, [searchParamsObj, router]);

    return (
        <div className="min-h-screen bg-base-300 flex flex-col items-center justify-center">
            <div className="bg-base-100 p-8 rounded-lg shadow-lg max-w-md w-full text-center">
                <h2 className="text-3xl font-bold mb-6">Email Confirmation</h2>
                {status === 'loading' && (
                    <div className="animate-pulse">
                        <p className="text-lg">Confirming your email...</p>
                    </div>
                )}
                {status === 'success' && (
                    <div>
                        <p className="text-green-500 text-lg mb-4">{message}</p>
                        <p className="text-base-content">Redirecting to login page...</p>
                    </div>
                )}
                {status === 'error' && (
                    <p className="text-red-500 text-lg">{message}</p>
                )}
            </div>
        </div>
    );
}