// components/HeroSection.tsx
import Link from 'next/link';

export const HeroSection = () => {
    return (
        <div className="hero min-h-screen" style={{ backgroundImage: `url('https://i.pinimg.com/originals/0d/10/d2/0d10d2fe48a7956a4fdc9f7251132236.gif')`}}>
            <div className="hero-overlay bg-black bg-opacity-70"></div>
            <div className="hero-content text-center text-neutral-content">
                <div className="min-w-64 px-6 py-12">
                    <h1 className="mb-10 text-8xl font-extrabold text-white" style={{ textShadow: '2px 2px 4px rgba(0, 0, 0, 0.5)' }}>
                        Elevate Your Code, One Challenge at a Time
                    </h1>
                    <p className="mb-10 text-4xl font-semibold text-gray-300" style={{ textShadow: '2px 2px 4px rgba(0, 0, 0, 0.5)' }}>
                        Sharpen your skills with targeted exercises designed to boost your coding prowess. Dive into our code kata and see your potential unfold.
                    </p>
                    <Link href={"/join"} className="btn btn-primary btn-lg">Sign Up Now</Link>
                </div>
            </div>
        </div>
    );
};
