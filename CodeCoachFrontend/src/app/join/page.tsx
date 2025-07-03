import { Register } from '@/components/join/Register';
import { Footer } from "@/components/common/Footer";
import AuthWrapper from "@/components/common/AuthWrapper";

export default function JoinPage() {
    return (
        <AuthWrapper>
            <div className="min-h-screen flex flex-col">
                <Register/> {/* Client component */}
                <Footer/>
            </div>
        </AuthWrapper>
    );
}
