import { Problems } from "@/components/problems/Problems";
import { Footer } from "@/components/common/Footer";
import { DashboardNavbar } from "@/components/common/DashboardNavbar";
import AuthWrapper from "@/components/common/AuthWrapper";



export default function ProblemsPage() {
    return (
        <AuthWrapper requireAuth={true}>
            <div className="flex flex-col h-screen overflow-hidden bg-gray-900">
                <DashboardNavbar/>
                <Problems/>

            </div>
        </AuthWrapper>
    );
}