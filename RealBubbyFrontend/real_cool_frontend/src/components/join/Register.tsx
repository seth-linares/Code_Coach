// src/components/join/Register.tsx
"use client";

import Link from 'next/link';
import InputField from '../common/InputField';
import useRegistration from '@/hooks/useRegistration'

import React from "react";

export function Register() {

    const {
        formData,
        error,
        validationErrors,
        success,
        isLoading,
        handleChange,
        handleSubmit
    } = useRegistration();



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