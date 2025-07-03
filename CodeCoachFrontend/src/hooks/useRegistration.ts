// src/hooks/useRegistration.ts

import { useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";
import { RegisterRequest, RegisterResponse } from "@/types"


function useRegistration() {
    const router = useRouter();
    const [formData, setFormData] = useState<RegisterRequest>({
        username: '',
        password: '',
        confirmPassword: '',
        emailAddress: '',
    });
    const [error, setError] = useState<string | null>(null);
    const [validationErrors, setValidationErrors] = useState<Record<string, string[]>>({});
    const [success, setSuccess] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();
        setError(null);
        setValidationErrors({});
        setSuccess(null);
        setIsLoading(true);

        try {
            const response = await axios.post<RegisterResponse>('https://www.codecoachapp.com/api/Users/Register', formData);
            setSuccess(response.data.message);
            setTimeout(() => router.push("/login"), 3000);

        } catch (err) {
            if (axios.isAxiosError(err) && err.response) {
                if (err.response.status === 400) {
                    if (err.response.data.errors) {
                        setValidationErrors(err.response.data.errors);
                    } else {
                        setError(err.response.data.title || 'An error occurred during registration');
                    }
                } else {
                    setError(err.response.data.title || err.message || 'An unexpected error occurred');
                }
            } else {
                setError('An unexpected error occurred');
            }
            console.error('Registration error: ', err);
        } finally {
            setIsLoading(false);
        }
    };

    return {
        formData,
        error,
        validationErrors,
        success,
        isLoading,
        handleChange,
        handleSubmit
    };
}

export default useRegistration;
