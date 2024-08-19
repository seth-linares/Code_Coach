// src/hooks/use2FA.ts

import { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import { TwoFactorStatus } from '@/types';

const use2FA = () => {
    const [status, setStatus] = useState<TwoFactorStatus | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const getStatus = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await axios.get<TwoFactorStatus>('https://www.codecoachapp.com/api/Users/2FAStatus', {
                withCredentials: true,
            });
            setStatus(response.data);
        } catch (err) {
            if (axios.isAxiosError(err)) {
                setError(err.response?.data?.message || err.message || 'Failed to fetch 2FA status');
            } else {
                setError('An unexpected error occurred');
            }
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        getStatus();
    }, [getStatus]);

    const enable2FA = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await axios.post<{ message: string }>('https://www.codecoachapp.com/api/Users/Enable2FA', {}, {
                withCredentials: true,
            });
            await getStatus(); // Refresh the status after enabling
            return response.data.message;
        } catch (err) {
            if (axios.isAxiosError(err)) {
                setError(err.response?.data?.message || err.message || 'Failed to enable 2FA');
            } else {
                setError('An unexpected error occurred');
            }
        } finally {
            setLoading(false);
        }
    }, [getStatus]);

    const verify2FA = useCallback(async (verificationCode: string) => {
        setLoading(true);
        setError(null);
        try {
            const response = await axios.post<{ message: string }>('https://www.codecoachapp.com/api/Users/VerifyAnd2FA',
                { code: verificationCode },
                { withCredentials: true }
            );
            await getStatus(); // Refresh the status after verifying
            return response.data.message;
        } catch (err) {
            if (axios.isAxiosError(err)) {
                setError(err.response?.data?.message || err.message || 'Failed to verify 2FA');
            } else {
                setError('An unexpected error occurred');
            }
        } finally {
            setLoading(false);
        }
    }, [getStatus]);

    const disable2FA = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await axios.post<{ message: string }>('https://www.codecoachapp.com/api/Users/Disable2FA', {}, {
                withCredentials: true,
            });
            await getStatus(); // Refresh the status after disabling
            return response.data.message;
        } catch (err) {
            if (axios.isAxiosError(err)) {
                setError(err.response?.data?.message || err.message || 'Failed to disable 2FA');
            } else {
                setError('An unexpected error occurred');
            }
        } finally {
            setLoading(false);
        }
    }, [getStatus]);

    return {
        status,
        loading,
        error,
        getStatus,
        enable2FA,
        verify2FA,
        disable2FA
    };
};

export default use2FA;