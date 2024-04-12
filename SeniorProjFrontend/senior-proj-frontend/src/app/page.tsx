import Navbar from "@/components/Navbar";
import {Hero} from "@/components";

export default function Home() {
  return (
      <div className="flex flex-col gap-4 bg-gray-700 min-h-screen">
          <Navbar />
          <Hero />
      </div>
  );
}
