// src/app/dashboard/page.tsx

import AuthWrapper from "@/components/common/AuthWrapper";
import {DashboardNavbar} from "@/components/common/DashboardNavbar";
import {Footer} from "@/components/common/Footer";
import DashboardPage from '@/components/dashboard/DashboardPage';

export default function DashboardWrapper() {
    return (
        <AuthWrapper requireAuth={true}>
            <div className="min-h-screen flex flex-col bg-base-300">
                <DashboardNavbar/>
                <div className="flex-grow overflow-hidden"> {/* Added overflow-hidden */}
                    <DashboardPage />
                </div>
                <Footer/>
            </div>
        </AuthWrapper>
    );
}