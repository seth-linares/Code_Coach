// src/components/ProfilePage.tsx
"use client";

import React from 'react';
import useUserStats from '@/hooks/useUserStats';
import use2FA from '@/hooks/use2FA';
import UserStatsCard from './UserStatsCard';
import TwoFactorSection from './TwoFactorSection';
import APIKeyManager from "@/components/profile/APIKeyManager";
import ProfileHeader from "@/components/profile/ProfileHeader";

const ProfilePage: React.FC = () => {
    const { stats, loading: statsLoading, error: statsError } = useUserStats();
    const { status, loading: twoFALoading, error: twoFAError, getStatus, enable2FA, verify2FA, disable2FA } = use2FA();

    return (
        <div className="container mx-auto p-4">
            <ProfileHeader/>
            {statsLoading ? (
                <div className="flex justify-center items-center h-screen">
                    <span className="loading loading-spinner loading-lg"></span>
                </div>
            ) : statsError ? (
                <div className="alert alert-error shadow-lg m-4">
                    <div className="flex flex-row items-center">
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            className="stroke-current flex-shrink-0 h-6 w-6 mr-2"
                            fill="none"
                            viewBox="0 0 24 24"
                        >
                            <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth="2"
                                d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"
                            />
                        </svg>
                        <span>{statsError}</span>
                    </div>
                </div>
            ) : (
                stats && <UserStatsCard stats={stats} />
            )}

            <APIKeyManager/>

            <TwoFactorSection
                status={status}
                enable2FA={enable2FA}
                verify2FA={verify2FA}
                disable2FA={disable2FA}
                getStatus={getStatus}
                loading={twoFALoading}
                error={twoFAError}
            />


        </div>
    );
};

export default ProfilePage;
