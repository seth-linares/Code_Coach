// src/app/problems/[id]/page.tsx

"use client"

import { useParams } from "next/navigation"
import CodeEditor from "@/components/problems/CodeEditor";
import useProblemDetails from "@/hooks/useProblemDetails";
import ChatComponent from "@/components/problems/ChatComponent";

export function Problems() {
    const params = useParams();
    const probID = parseInt(params.id as string, 10);
    const { problemDetails, loading, error } = useProblemDetails(probID);

    if (loading) return <div className="flex items-center justify-center h-screen">Loading...</div>;
    if (error) return <div className="flex items-center justify-center h-screen">Error: {error}</div>;
    if (!problemDetails) return <div className="flex items-center justify-center h-screen">No problem details found</div>;

    return (
        <div className="flex flex-col md:flex-row h-screen overflow-hidden">
            <div className="w-full md:w-1/2 p-4 overflow-auto">
                <CodeEditor
                    problemId={probID}
                    languageDetails={problemDetails.languageDetails}
                />
            </div>

            <div className="w-full md:w-1/2 p-4 flex flex-col overflow-hidden">
                <div className="bg-base-100 p-4 rounded-lg mb-4 overflow-auto">
                    <h1 className="text-2xl font-bold mb-2 text-center">{problemDetails.title}</h1>
                    <pre className="whitespace-pre-wrap">{problemDetails.description}</pre>
                </div>

                <div className="flex-grow overflow-hidden">
                    <ChatComponent problemId={probID} />
                </div>
            </div>
        </div>
    );
}