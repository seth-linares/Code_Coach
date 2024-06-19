import Image from "next/image";
import Link from "next/link";
import {Navbar} from "../components/Navbar";
import {Footer} from "../components/Footer";
import {HeroSection} from "../components/HeroSection";

export default function Home() {
  return (
    <main data-theme={"dracula"}>
      <Navbar/>
      <HeroSection/>
      <Footer/>
    </main>
  );
}
