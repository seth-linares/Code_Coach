// src/app/problems/[id]/page.tsx

"use client"

import { useParams } from "next/navigation"
import CodeEditor from "@/components/problems/CodeEditor";
import useProblemDetails from "@/hooks/useProblemDetails";

export function Problems() {
    const params = useParams();
    const probID = parseInt(params.id as string, 10);
    const { problemDetails, loading, error } = useProblemDetails(probID);

    if (loading) return <div>Loading...</div>;
    if (error) return <div>Error: {error}</div>;
    if (!problemDetails) return <div>No problem details found</div>;

    console.log(`DESCRIPTION: ${problemDetails.description}`);

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

                <div className="flex-grow flex flex-col">
                    <div className="chat-container flex-grow overflow-y-auto mb-2 p-4 bg-base-200 rounded-t-lg">
                        {/* Chat messages will go here */}
                    </div>
                    <div className="input-container">
                        <input type="text" placeholder="Chat with AI..." className="input input-bordered w-full"
                               id="user-input"/>
                    </div>
                </div>
            </div>
        </div>
    );
}
