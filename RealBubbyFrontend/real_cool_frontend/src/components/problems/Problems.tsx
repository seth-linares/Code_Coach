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

    if (loading) return <div>Loading...</div>;
    if (error) return <div>Error: {error}</div>;
    if (!problemDetails) return <div>No problem details found</div>;

    return (
        <div className="flex-grow bg-blend-color flex flex-col md:flex-row p-4 gap-4">
            <div className="w-full md:w-1/2 flex flex-col">
                <CodeEditor
                    problemId={probID}
                    languageDetails={problemDetails.languageDetails}
                />
            </div>

            <div className="w-full md:w-1/2 flex flex-col">
                <div className="bg-base-100 p-4 rounded-lg mb-4">
                    <h1 className="text-2xl font-bold mb-2 text-center">{problemDetails.title}</h1>
                    <pre className="whitespace-pre-wrap">{problemDetails.description}</pre>
                </div>

                <ChatComponent problemId={probID} />
            </div>
        </div>
    );
}