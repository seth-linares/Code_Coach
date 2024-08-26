// src/app/problems/[id]/page.tsx

"use client"

import { useParams } from "next/navigation"
import CodeEditor from "@/components/problems/CodeEditor";
import useProblemDetails from "@/hooks/useProblemDetails";
import ChatComponent from "@/components/problems/ChatComponent";
import ReactMarkdown from "react-markdown";

export function Problems() {
    const params = useParams();
    const probID: number = parseInt(params.id as string, 10);
    const { problemDetails, loading, error } = useProblemDetails(probID);

    if (loading) return <div className="flex items-center justify-center h-screen">Loading...</div>;
    if (error) return <div className="flex items-center justify-center h-screen">Error: {error}</div>;
    if (!problemDetails) return <div className="flex items-center justify-center h-screen">No problem details found</div>;

    return (
        <div className="flex flex-col lg:flex-row h-screen overflow-y-auto overflow-x-auto">
            <div className="w-full lg:w-1/2 p-4 flex flex-col h-full">
                <div className="bg-base-100 p-4 rounded-lg mb-4 flex-shrink-0 overflow-y-auto max-h-[30vh] lg:max-h-[40vh]">
                    <h1 className="text-2xl font-bold mb-2 text-center">{problemDetails.title}</h1>
                    <ReactMarkdown 
                        className="whitespace-pre-wrap prose max-w-none" 
                    >
                        {problemDetails.description}
                    </ReactMarkdown>
                </div>
                <div className="flex-grow overflow-hidden">
                    <CodeEditor
                        problemId={probID}
                        languageDetails={problemDetails.languageDetails}
                    />
                </div>
            </div>

            <div className="w-full lg:w-1/2 p-4 h-full">
                <ChatComponent problemId={probID} />
            </div>
        </div>
    );
}