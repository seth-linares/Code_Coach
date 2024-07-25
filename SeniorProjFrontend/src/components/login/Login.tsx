// src/components/auth/Login.tsx
"use client";

import React from 'react';
import useLogin from '@/hooks/useLogin';
import InputField from '../common/InputField';
import TwoFactorVerification from '../auth/TwoFactorVerification';
import Link from "next/link";

const Login: React.FC = () => {
    const {
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
    } = useLogin();

    return (
        <div className="flex-grow bg-base-300 flex flex-col items-center justify-center py-10">
            <div className="bg-base-100 p-8 rounded-lg shadow-lg max-w-md w-full">
                <h2 className="text-2xl font-bold mb-6 text-center">
                    {requiresTwoFactor ? '2FA Verification' : 'Login'}
                </h2>

                {!requiresTwoFactor ? (
                    <form onSubmit={handleSubmit} className="space-y-4">
                        <InputField
                            label="Username"
                            type="text"
                            name="username"
                            value={formData.username}
                            onChange={handleChange}
                            required
                        />
                        <InputField
                            label="Password"
                            type="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            required
                        />
                        <InputField
                            label="Remember Me"
                            type="checkbox"
                            name="rememberMe"
                            value={formData.rememberMe}
                            onChange={handleChange}
                        />
                        <button type="submit" className="btn btn-primary w-full" disabled={isLoading}>
                            {isLoading ? 'Logging in...' : 'Log In'}
                        </button>
                        {error && <div className="alert alert-error mt-4">{error}</div>}
                        {success && <div className="alert alert-success mt-4">{success}</div>}
                    </form>
                ) : (
                    <TwoFactorVerification
                        twoFactorData={twoFactorData}
                        onChange={handleTwoFactorChange}
                        onSubmit={handleTwoFactorSubmit}
                        isLoading={isLoading}
                        error={error}
                    />
                )}
            </div>
            <p className="text-center mt-6">
                Don&apos;t have an account yet? <Link href={"/join"} className="link link-primary">Join CodeCoach!</Link>
            </p>
        </div>
    );
};

export default Login;