import Image from "next/image";
import Link from "next/link";
import {Register} from '@/components/join/Register';
import { Footer } from "@/components/common/Footer";

export default function JoinPage() {
    return (
        <div className="min-h-screen flex flex-col">
            <Register/> {/* Client component */}
            <Footer/>
        </div>
    );
}
