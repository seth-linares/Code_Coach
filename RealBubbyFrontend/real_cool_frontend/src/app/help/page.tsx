// src/app/help/page.tsx


import Link from "next/link";
import { DashboardNavbar } from "@/components/common/DashboardNavbar";

interface FAQItem {
    question: string;
    answer: string;
}

const faqItems: FAQItem[] = [
    {
        question: "What is CodeCoach?",
        answer: "CodeCoach is an edutech platform designed to help students learn coding through personalized coaching and interactive coding problems."
    },
    {
        question: "How do I get started with CodeCoach?",
        answer: "To get started, simply sign up for an account on our homepage and choose a learning path that suits your goals and current skill level."
    },
    {
        question: "What programming languages do you teach?",
        answer: "We offer coding problems in languages like Python, C++, and C#."
    },
    {
        question: "Is CodeCoach suitable for beginners?",
        answer: "Absolutely! We have courses designed for all skill levels, from complete beginners to advanced programmers looking to expand their knowledge."
    },
    {
        question: "How much does CodeCoach cost?",
        answer: "We are currently in beta! Feel free to use the platform as you please. The only costs that are going to be associated with this site are going to be with the AI tutor API key which you must provide yourself. However, costs should be incredibly low!"
    },
    {
        question: "How do I set up the AI tutor?",
        answer: "To use the AI tutor feature, you must add your OpenAI API key in the API Keys section of your user profile. Without this key, the AI tutor functionality will not be available. Visit your profile page to add your API key."
    }
];

export default function HelpPage() {
    return (
        <div>
            <DashboardNavbar />
            <main className="flex min-h-screen flex-col items-center p-8 bg-base-300">

                <h1 className="text-4xl font-bold mb-8">Help & FAQ</h1>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 w-full max-w-4xl">
                    {faqItems.map((item, index) => (
                        <div key={index} className="card bg-base-100 shadow-xl hover:scale-105 transition-transform duration-300">
                            <div className="card-body">
                                <h2 className="card-title">{item.question}</h2>
                                <p>{item.answer}</p>
                                {item.question === "How do I set up the AI tutor?" && (
                                    <div className="card-actions justify-end mt-4">
                                        <Link href={"/profile"} className="btn btn-primary">
                                            Go to Profile
                                        </Link>
                                    </div>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
                <div className="mt-12 text-center">
                    <h2 className="text-2xl font-semibold mb-4">Need more help?</h2>
                    <p className="mb-4">If you couldn&apos;t find the answer you were looking for, check out our resources page for more information.</p>
                    <Link href="https://www.codecoachapp.com/resources" className="btn btn-secondary">
                        View Resources
                    </Link>
                </div>
            </main>
        </div>
    );
}