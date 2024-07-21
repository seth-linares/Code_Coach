// src/app/dashboard/page.tsx

import Link from "next/link";
import AuthWrapper from "@/components/common/AuthWrapper";
import {Dashboard} from "@/components/dashboard/Dashboard";
import {DashboardNavbar} from "@/components/common/DashboardNavbar";
import {Footer} from "@/components/common/Footer";

export default function DashboardPage() {
    return (
        <AuthWrapper requireAuth={true}>
        <div className="min-h-screen flex flex-col">
            <DashboardNavbar/>
            <Dashboard/>
            <Footer/>
        </div>
        </AuthWrapper>
    );
}