import Image from "next/image";
import Link from "next/link";
import {Register} from '@/components/Register';
import {Footer} from '@/components/Footer';

export default function Home() {
    return (
        <main>
            <Register/>
            <Footer/>
        </main>

    );
}
