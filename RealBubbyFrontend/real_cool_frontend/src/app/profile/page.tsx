// src/app/profile/page.tsx

import dynamic from 'next/dynamic';
import AuthWrapper from "@/components/common/AuthWrapper";
import { Footer } from '@/components/common/Footer';

const ProfilePage = dynamic(() => import('@/components/profile/ProfilePage'), {
    ssr: false,
    loading: () => <p>Loading...</p>
});

export default function ProfileWrapper() {
    return (
        <AuthWrapper requireAuth={true}>
            <div className="bg-gray-900 min-h-screen flex flex-col">
                <div className="flex-grow">
                    <ProfilePage />
                </div>
                <Footer/>
            </div>
        </AuthWrapper>
    );
}