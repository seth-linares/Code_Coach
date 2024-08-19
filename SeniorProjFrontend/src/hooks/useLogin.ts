// src/hooks/useLogin.ts


import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import axios from 'axios';

export interface LoginRequest {
    username: string;
    password: string;
    rememberMe: boolean;
}

export interface TwoFactorVerificationRequest {
    code: string;
    rememberMe: boolean;
    rememberBrowser: boolean;
}

export interface LoginResponse {
    requiresTwoFactor?: boolean;
    message: string;
}

export function useLogin() {
    const router = useRouter();

    const [formData, setFormData] = useState<LoginRequest>({
        username: "",
        password: "",
        rememberMe: false,
    });

    const [twoFactorData, setTwoFactorData] = useState<TwoFactorVerificationRequest>({
        code: "",
        rememberMe: false,
        rememberBrowser: false,
    });

    const [error, setError] = useState<string | null>(null);
    const [requiresTwoFactor, setRequiresTwoFactor] = useState<boolean>(false);
    const [success, setSuccess] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
        const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value;
        setFormData({ ...formData, [e.target.name]: value });
    }

    const handleTwoFactorChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
        const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value;
        setTwoFactorData({ ...twoFactorData, [e.target.name]: value });
    }

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();
        setError(null);
        setSuccess(null);
        setRequiresTwoFactor(false);
        setIsLoading(true);

        try {
            const response = await axios.post<LoginResponse>('https://www.codecoachapp.com/api/Users/Login', formData);

            if (response.data.requiresTwoFactor) {
                setRequiresTwoFactor(true);
                setSuccess(response.data.message);
            } else {
                setSuccess(response.data.message);
                router.push("/dashboard");
            }
        } catch (err) {
            handleError(err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleTwoFactorSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();
        setError(null);
        setSuccess(null);
        setIsLoading(true);

        try {
            const response = await axios.post<LoginResponse>('https://www.codecoachapp.com/api/Users/VerifyTwoFactorCode', twoFactorData);
            setSuccess(response.data.message);
            setRequiresTwoFactor(false);
            router.push("/dashboard");
        } catch (err) {
            handleError(err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleError = (err: unknown) => {
        if (axios.isAxiosError(err) && err.response) {
            switch (err.response.status) {
                case 400:
                    setError(err.response.data.message || 'Bad request. Please check your input.');
                    break;
                case 401:
                    setError(err.response.data.message || 'Invalid username or password.');
                    break;
                case 423:
                    setError(err.response.data.message || 'Account is locked. Please try again later.');
                    break;
                case 500:
                    setError(err.response.data.message || 'An unexpected error occurred. Please try again later.');
                    break;
                default:
                    setError('An unexpected error occurred. Please try again later.');
            }
        } else {
            setError('An unexpected error occurred. Please try again later.');
        }
        console.error('Login error:', err);
    };

    return {
        formData,
        twoFactorData,
        error,
        requiresTwoFactor,
        success,
        isLoading,
        handleChange,
        handleTwoFactorChange,
        handleSubmit,
        handleTwoFactorSubmit,
    };
}

export default useLogin;