// hooks/useRegister.ts

import { useState } from 'react';
import axios from 'axios';
import { RegisterRequest, RegisterResponse, ValidationError } from '@/pages/api/register';

interface RegisterHook {
    register: (data: RegisterRequest) => Promise<void>;
    isLoading: boolean;
    error: string | null;
    validationErrors: ValidationError | null;
    success: boolean;
}

export const useRegister = (): RegisterHook => {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [validationErrors, setValidationErrors] = useState<ValidationError | null>(null);
    const [success, setSuccess] = useState(false);

    const register = async (data: RegisterRequest) => {
        setIsLoading(true);
        setError(null);
        setValidationErrors(null);
        setSuccess(false);

        try {
            const response = await axios.post<RegisterResponse>('/api/Users/Register', data);
            setSuccess(true);
            // You might want to do something with the response, like storing the userId
            console.log(response.data.message);
        } catch (err) {
            if (axios.isAxiosError(err) && err.response) {
                if (err.response.status === 400) {
                    setValidationErrors(err.response.data.errors);
                } else {
                    setError(err.response.data.message || 'An error occurred during registration');
                }
            } else {
                setError('An unexpected error occurred -- breaking in the setError');
            }
        } finally {
            setIsLoading(false);
        }
    };

    return { register, isLoading, error, validationErrors, success };
};