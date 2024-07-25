// src/common/components/Footer.tsx
import { FaGithub } from 'react-icons/fa';  // Importing GitHub icon from react-icons
import Link from 'next/link';

export const Footer = () => {
    return (
        <footer className="bg-neutral text-neutral-content p-4">
            <div className="container mx-auto flex flex-wrap items-center justify-between">
                <div className="flex items-center">
                    <Link href="/" className="text-lg font-bold">
                        <span className="text-primary text-2xl font-extrabold tracking-tight" style={{ textShadow: '1px 1px 2px rgba(0, 0, 0, 0.5)' }}>
                            Code<span className="text-neutral-content">Coach</span>
                        </span>
                    </Link>
                </div>
                <div className="flex gap-4 items-center">
                    <Link href="https://github.com/seth-linares" target="_blank" className="hover:text-primary transition-colors flex items-center gap-2">
                        <FaGithub className="text-lg" />
                        GitHub
                    </Link>
                    <Link href={"/help"} className="hover:text-primary transition-colors">
                        Help/FAQ
                    </Link>
                    <Link href={"/resources"} className="hover:text-primary transition-colors">
                        Resources
                    </Link>
                </div>
            </div>
        </footer>
    );
};
