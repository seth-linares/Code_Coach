// src/app/resources/page.tsx

import { DashboardNavbar } from "@/components/common/DashboardNavbar";
import Link from "next/link";

interface Resource {
    title: string;
    description: string;
    url: string;
}

const resources: Resource[] = [
    {
        title: "C# Documentation",
        description: "Official Microsoft documentation for C#, including language tour and overview.",
        url: "https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/overview"
    },
    {
        title: "Python Documentation",
        description: "Official Python documentation for version 3.11, covering language basics and advanced topics.",
        url: "https://docs.python.org/3.11/"
    },
    {
        title: "C++ Documentation",
        description: "Comprehensive C++ documentation covering language features, standard library, and more.",
        url: "https://devdocs.io/cpp/"
    },
    {
        title: "OpenAI API Documentation",
        description: "Learn how to obtain and use an API key for OpenAI's services, essential for CodeCoach's AI tutor feature.",
        url: "https://platform.openai.com/docs/overview"
    }
];

export default function ResourcesPage() {
    return (
        <div className="min-h-screen flex flex-col">
            <DashboardNavbar />
            <main className="flex-grow flex flex-col items-center p-8 pt-16 bg-base-300">
                <div className="w-full max-w-6xl">
                    <div className="mb-12">
                        <h1 className="text-4xl font-bold text-center">Learning Resources</h1>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-6">
                        {resources.map((resource, index) => (
                            <div key={index} className="card bg-base-100 shadow-xl hover:scale-105 transition-transform duration-300">
                                <div className="card-body">
                                    <h2 className="card-title">{resource.title}</h2>
                                    <p>{resource.description}</p>
                                    <div className="card-actions justify-end mt-4">
                                        <a href={resource.url} target="_blank" rel="noopener noreferrer" className="btn btn-primary">
                                            Visit Resource
                                        </a>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                    <div className="mt-12 text-center">
                        <p className="mb-4">These resources will help you deepen your understanding of the programming languages covered in CodeCoach and set up your AI tutor feature.</p>
                        <Link href={"/help"} className="btn btn-secondary">
                            Need Help?
                        </Link>
                    </div>
                </div>
            </main>
        </div>
    );
}