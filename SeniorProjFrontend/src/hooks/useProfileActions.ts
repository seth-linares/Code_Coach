// https://www.codecoachapp.com/api/Users/ChangePassword

// src/hooks/useChangePassword.ts


import { useState, useCallback } from 'react';
import axios from 'axios';
import { useRouter } from 'next/navigation';

interface ChangePasswordDto {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

interface UseProfileActionsResult {
    loading: boolean;
    error: string | null;
    message: string | null;
    validationErrors: Record<string, string[]> | null;
    changePassword: (passwordDto: ChangePasswordDto) => Promise<boolean>;
    logout: () => Promise<boolean>;
}

export const useProfileActions = (): UseProfileActionsResult => {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [message, setMessage] = useState<string | null>(null);
    const [validationErrors, setValidationErrors] = useState<Record<string, string[]> | null>(null);
    const router = useRouter();

    const changePassword = useCallback(async (passwordDto: ChangePasswordDto): Promise<boolean> => {
        setLoading(true);
        setError(null);
        setMessage(null);
        setValidationErrors(null);
        try {
            const response = await axios.post('https://www.codecoachapp.com/api/Users/ChangePassword', passwordDto, {
                withCredentials: true,
            });
            setMessage(response.data);
            console.log('Password change successful:', response.data);
            return true; // Password change was successful
        } catch (err) {
            if (axios.isAxiosError(err)) {
                if (err.response) {
                    if (err.response.status === 400 && err.response.data.errors) {
                        setValidationErrors(err.response.data.errors);
                    } else {
                        const errorMessage = err.response.data?.title || err.response.data || err.message;
                        setError(errorMessage);
                    }
                } else {
                    setError(err.message);
                }
                console.error('Password change error:', err);
            } else {
                setError('An unexpected error occurred');
                console.error('Password change unexpected error:', err);
            }
            return false; // Password change failed
        } finally {
            setLoading(false);
        }
    }, []);

    const logout = useCallback(async (): Promise<boolean> => {
        setLoading(true);
        setError(null);
        setMessage(null);
        try {
            const response = await axios.post('https://www.codecoachapp.com/api/Users/Logout', {}, {
                withCredentials: true,
            });
            setMessage(response.data.message);
            console.log('Logout successful:', response.data.message);

            // Redirect to the login page after successful logout
            router.push('/login');

            return true;
        } catch (err) {
            if (axios.isAxiosError(err)) {
                const errorMessage = err.response?.data?.title || err.response?.data || err.message;
                setError(errorMessage);
                console.error('Logout error:', errorMessage);
            } else {
                setError('An unexpected error occurred');
                console.error('Logout unexpected error:', err);
            }
            return false;
        } finally {
            setLoading(false);
        }
    }, [router]);

    return { loading, error, message, validationErrors, changePassword, logout };
};

