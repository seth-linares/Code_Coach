// src/components/auth/TwoFactorVerification.tsx
"use client";

import React from 'react';
import InputField from '../common/InputField';

interface TwoFactorVerificationProps {
    twoFactorData: {
        code: string;
        rememberMe: boolean;
        rememberBrowser: boolean;
    };
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    onSubmit: (e: React.FormEvent<HTMLFormElement>) => void;
    isLoading: boolean;
    error: string | null;
}

const TwoFactorVerification: React.FC<TwoFactorVerificationProps> = ({
     twoFactorData,
     onChange,
     onSubmit,
     isLoading,
     error
 }) => {
    return (
        <form onSubmit={onSubmit} className="space-y-4">
            <InputField
                label="2FA Code"
                type="text"
                name="code"
                value={twoFactorData.code}
                onChange={onChange}
                required
            />
            <InputField
                label="Remember Me"
                type="checkbox"
                name="rememberMe"
                value={twoFactorData.rememberMe}
                onChange={onChange}
            />
            <InputField
                label="Remember this browser"
                type="checkbox"
                name="rememberBrowser"
                value={twoFactorData.rememberBrowser}
                onChange={onChange}
            />
            <button type="submit" className="btn btn-primary w-full" disabled={isLoading}>
                {isLoading ? 'Verifying...' : 'Verify 2FA Code'}
            </button>
            {error && <div className="alert alert-error">{error}</div>}
        </form>
    );
};

export default TwoFactorVerification;