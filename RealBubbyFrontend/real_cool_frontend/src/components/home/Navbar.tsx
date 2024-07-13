// src/components/home/Navbar.tsx
import Link from 'next/link';

export const Navbar = () => (
    <nav className="fixed z-50 w-full bg-base-100 border-b border-gray-500 h-14 flex items-center">
        <div className="container mx-auto px-4 flex justify-between items-center">
            <Link href={"/"} className="btn btn-ghost normal-case text-xl">
                <span className="font-bold text-4xl tracking-tight text-primary hover:text-secondary transition-colors duration-300">
                    Code<span className="text-neutral-content">Coach</span>
                </span>
            </Link>
            <ul className="menu menu-horizontal space-x-2.5">
                <li><Link href={"/"} className="btn btn-outline btn-primary hover:bg-primary hover:text-white">Home</Link></li>
                <li><Link href={"/login"} className="btn btn-outline btn-secondary hover:bg-secondary hover:text-white">Login</Link></li>
                <li><Link href={"/help"} className="btn btn-outline btn-accent hover:bg-accent hover:text-white">Help/FAQ</Link></li>
                <li><Link href={"/resources"} className="btn btn-outline btn-info hover:bg-info hover:text-white">Resources</Link></li>
            </ul>
        </div>
    </nav>
);

