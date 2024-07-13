import Image from "next/image";
import Link from "next/link";

import {Navbar} from "@/components/home/Navbar";
import {HeroSection} from "@/components/home/HeroSection";
import {FeaturesSection} from "@/components/home/FeaturesSection";
import {Footer} from "@/components/common/Footer";
import {HowItWorksSection} from "@/components/home/HowItWorksSection";
import AuthWrapper from "@/components/common/AuthWrapper";

export default function Home() {

  return (
      <AuthWrapper>
        <main className={"bg-base-100"}>
          <Navbar/>
          <HeroSection/>
          <FeaturesSection/>
          <HowItWorksSection/> {/* Client component */}
          <Footer/>
        </main>
      </AuthWrapper>
  );
}
