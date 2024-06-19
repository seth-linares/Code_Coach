import Link from "next/link";
import Image from "next/image";

export const Footer = () => {
    return(
        <nav>
            <div>
                <Link href="/">
                    <Image
                        src={"/cropped_codecoach.png"} alt="Cropped Codecoach"
                        width={50}
                        height={20}
                    />
                </Link>
            </div>

            <div>
                <ul>
                    <li><Link href={"/login"}>Login</Link></li>
                    <li><Link href={"/help"}>Help/FAQ</Link></li>
                    <li><Link href={"/resources"}>Resources</Link></li>
                </ul>
            </div>
        </nav>
    )
}