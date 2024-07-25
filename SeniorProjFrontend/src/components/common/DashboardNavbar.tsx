import Link from "next/link";

export const DashboardNavbar = () => (
    <nav className="z-50 w-full bg-base-100 border-b border-gray-500 h-14 flex items-center">
        <div className="container mx-auto px-4 flex justify-between items-center">
            <Link href={"/dashboard"} className="btn btn-ghost normal-case text-xl">
                <span className="font-bold text-4xl tracking-tight text-primary hover:text-secondary transition-colors duration-300">
                    Code<span className="text-neutral-content">Coach</span>
                </span>
            </Link>
            <ul className="menu menu-horizontal space-x-2.5">
                <li><Link href={"/dashboard"} className="btn btn-outline btn-primary hover:bg-primary hover:text-white">Dashboard</Link></li>
                <li><Link href={"/profile"} className="btn btn-outline btn-accent hover:bg-accent hover:text-white">Profile</Link></li>
                <li><Link href={"/resources"} className="btn btn-outline btn-info hover:bg-info hover:text-white">Resources</Link></li>
            </ul>
        </div>
    </nav>
);

