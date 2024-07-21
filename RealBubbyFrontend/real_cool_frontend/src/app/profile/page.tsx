// src/app/profile/page.tsx
'use client';

import dynamic from 'next/dynamic';
import AuthWrapper from "@/components/common/AuthWrapper";

const ProfilePage = dynamic(() => import('@/components/profile/ProfilePage'), {
    ssr: false,
    loading: () => <p>Loading...</p>
});

export default function ProfileWrapper() {
    return(
        <AuthWrapper requireAuth={true}>
            <div className="bg-gray-900">
                <ProfilePage />
            </div>

        </AuthWrapper>
    );
}