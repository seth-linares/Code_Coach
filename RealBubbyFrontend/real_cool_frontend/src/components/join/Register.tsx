// src/components/join/Register.tsx


// TODO:
//  1. JWT to local storage and
//  2. re-route to real profile page
"use client";

import React, { useState } from 'react';
import Link from 'next/link';
import InputField from '../common/InputField';
import { useRouter } from "next/navigation";
import {AuthResponse, register, RegisterRequest} from '@/services/authService';
import {ApiResponse} from "@/services/api";
import {RegisterResponse} from "@/pages/api/register";
import {AppRouterInstance} from "next/dist/shared/lib/app-router-context.shared-runtime";

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

    const router: AppRouterInstance = useRouter();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();
        setError(null);
        setValidationErrors({});
        setSuccess(null);

        const result: ApiResponse<AuthResponse> = await register(formData);

        if (result.data) {
            setSuccess('Registration successful!');
            router.push('/dashboard');
        } else if (result.validationErrors) {
            setValidationErrors(result.validationErrors);
        } else if (result.error) {
            setError(result.error);
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
                    error={validationErrors.Username?.[0]}
                    required
                />
                <InputField
                    label="Email"
                    type="email"
                    name="emailAddress"
                    value={formData.emailAddress}
                    onChange={handleChange}
                    error={validationErrors.EmailAddress?.[0]}
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
                    <button type="submit" className="btn btn-primary w-full">Register</button>
                </div>
            </form>

            <p className="text-center mt-6">
                Already have an account? <Link href={"/login"} className="link link-primary">Login</Link>
            </p>
        </div>
    );
}
