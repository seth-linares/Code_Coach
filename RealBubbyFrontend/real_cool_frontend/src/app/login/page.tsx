import Image from "next/image";
import Link from "next/link";

export default function Home() {

    return (
        <main className="flex min-h-screen flex-col items-center justify-between p-24">
            <Link href="/" className="btn font-bold text-xl">
                Home
            </Link>
            <h1>Login Bubby!</h1>


        </main>
    );
}
