"use client"

import { useRef, useState } from "react";
import Editor, { OnMount } from "@monaco-editor/react";

const languageMap: { [key: string]: string } = { 'Python': 'python', 'C++': 'cpp', 'C#': 'csharp' };

export function Problems() {
    const [selectedLanguage, setSelectedLanguage] = useState("Python");
    const editorRef = useRef<any>(null);

    const handleEditorDidMount: OnMount = (editor, monaco) => {
        editorRef.current = editor;
    };

    return (
        <div className="flex-grow bg-blend-color flex flex-col md:flex-row p-4 gap-4">
            <div className="w-full md:w-1/2 flex flex-col">
                <div className="flex-grow">
                    <Editor
                        height="60vh"
                        language={languageMap[selectedLanguage]}
                        theme="vs-dark"
                        onMount={handleEditorDidMount}
                        className="rounded-t-lg"
                    />
                </div>
                <div className="flex justify-start items-center mt-2">
                    <div className="dropdown">
                        <div tabIndex={0} role="button" className="btn btn-sm m-1">{selectedLanguage}</div>
                        <ul tabIndex={0}
                            className="dropdown-content menu bg-base-100 rounded-box z-[1] w-52 p-2 shadow">
                            <li><a onClick={() => setSelectedLanguage('Python')}>Python</a></li>
                            <li><a onClick={() => setSelectedLanguage('C++')}>C++</a></li>
                            <li><a onClick={() => setSelectedLanguage('C#')}>C#</a></li>
                        </ul>
                    </div>
                    <button className="btn btn-sm ml-2">Submit</button>
                </div>
            </div>

            <div className="w-full md:w-1/2 flex flex-col">
                <div className="bg-base-100 p-4 rounded-lg mb-4">
                    <h1 className="text-2xl font-bold mb-2 text-center">Title Here</h1>
                    <p>Description Here</p>
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