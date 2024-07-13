// src/app/login/page.tsx

import Login from "@/components/login/Login";
import { Footer } from "@/components/common/Footer";
import AuthWrapper from "@/components/common/AuthWrapper";



export default function LoginPage() {
    return (
        <AuthWrapper>
            <div className="min-h-screen flex flex-col">
                <Login/>
                <Footer/>
            </div>
        </AuthWrapper>
    );
}
