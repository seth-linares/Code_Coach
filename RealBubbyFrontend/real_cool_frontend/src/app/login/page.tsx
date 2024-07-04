import Image from "next/image";
import Link from "next/link";
import { Login } from "@/components/login/Login";
import { Footer } from "@/components/common/Footer";



export default function LoginPage() {
    return (
        <div className="min-h-screen flex flex-col">
            <Login/>
            <Footer/>
        </div>
    );
}
