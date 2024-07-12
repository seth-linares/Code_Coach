// src/components/join/Register.tsx


"use client";

import React, { useState } from 'react';
import Link from 'next/link';

import InputField from '../common/InputField';
import { RegisterRequest, RegisterResponse } from "@/pages/api/register";
import axios, { AxiosError } from 'axios';

export function Register() {
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
            const response = await axios.post<RegisterResponse>('https://localhost/api/Users/Register', formData);
            setSuccess(response.data.message);
        } catch (err) {
            if (axios.isAxiosError(err) && err.response) {
                if (err.response.status === 400) {
                    if (err.response.data.errors) {
                        // Handle validation errors
                        setValidationErrors(err.response.data.errors);
                    } else {
                        // Handle other 400 errors
                        setError(err.response.data.title || 'An error occurred during registration');
                    }
                } else {
                    // Handle non-400 errors
                    setError(err.response.data.title || err.message || 'An unexpected error occurred');
                }
            } else {
                setError('An unexpected error occurred');
            }
            console.error('Registration error:', err);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="flex-grow bg-base-300 flex flex-col items-center justify-center py-10">
            <form onSubmit={handleSubmit} className="form bg-base-100 p-6 rounded-lg shadow-lg max-w-md w-full">
                <h2 className="text-2xl font-bold mb-6 text-center">Register</h2>

                {error && <div className="alert alert-error mb-4">{error}</div>}
                {success && <div className="alert alert-success mb-4">{success}</div>}

                <InputField
                    label="Username"
                    type="text"
                    name="username"
                    value={formData.username}
                    onChange={handleChange}
                    error={validationErrors.UserName?.[0]}
                    required
                />
                <InputField
                    label="Email"
                    type="email"
                    name="emailAddress"
                    value={formData.emailAddress}
                    onChange={handleChange}
                    error={validationErrors.Email?.[0]}
                    required
                />
                <InputField
                    label="Password"
                    type="password"
                    name="password"
                    value={formData.password}
                    onChange={handleChange}
                    error={validationErrors.Password?.[0]}
                    required
                />
                <InputField
                    label="Confirm Password"
                    type="password"
                    name="confirmPassword"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    error={validationErrors.ConfirmPassword?.[0]}
                    required
                />

                <div className="form-control mt-6">
                    <button type="submit" className="btn btn-primary w-full" disabled={isLoading}>
                        {isLoading ? 'Registering...' : 'Register'}
                    </button>
                </div>
            </form>

            <p className="text-center mt-6">
                Already have an account? <Link href={"/login"} className="link link-primary">Login</Link>
            </p>
        </div>
    );
}