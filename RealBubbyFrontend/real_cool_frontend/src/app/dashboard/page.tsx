// src/app/dashboard/page.tsx

import Link from "next/link";
import AuthWrapper from "@/components/common/AuthWrapper";

export default function DashboardPage() {
    return (
        <AuthWrapper requireAuth={true}>
            <main className="flex min-h-screen flex-col items-center justify-between p-24">
                <Link href="/" className="btn font-bold text-xl">
                    Home
                </Link>
                <h1>Dashboard Bubby!</h1>
            </main>
        </AuthWrapper>
    );
}