import Link from "next/link";

type Props = {};
export default function Navbar({}: Props) {
    return (
        <header className='w-full absolute top-0 left-0 z-10'>
            <nav className='max-w-[1440px] mx-auto flex justify-between items-center sm:px-16 px-6 py-4 bg-transparent'>
                <Link href="/" className="flex items-center justify-center gap-2">
                    <h2 className="text-white text-3xl">CodeCoach</h2>
                </Link>

                <section className="flex gap-2">
                    <button className="login__btn">log in</button>
                    <button className="signup__btn">sign up</button>
                </section>
            </nav>
        </header>
    );
}