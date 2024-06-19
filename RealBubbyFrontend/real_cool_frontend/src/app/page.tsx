import Image from "next/image";
import Link from "next/link";

import {Navbar} from "@/components/Navbar";
import {HeroSection} from "@/components/HeroSection";
import {FeaturesSection} from "@/components/FeaturesSection";
import {Footer} from "@/components/Footer";
import {HowItWorksSection} from "@/components/HowItWorksSection";

export default function Home() {
  return (
    <main className={"bg-base-100"}>
      <Navbar/>
      <HeroSection/>
      <FeaturesSection/>
      <HowItWorksSection/>
      {/*<Footer/>*/}
    </main>
  );
}
