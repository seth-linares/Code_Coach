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
        <div className="flex-grow bg-base-300 flex flex-row items-center justify-center py-10 pl-10">
            <div className="h-[1100px] w-1/2 flex flex-col items-center">
                <Editor
                    height="100%"
                    width="100%"
                    language={languageMap[selectedLanguage]}
                    theme="vs-dark"
                    onMount={handleEditorDidMount}
                    className="mb-4"
                />
                <div className="flex justify-center items-center w-full">
                    <div className="dropdown">
                        <div tabIndex={0} role="button" className="btn m-1">{selectedLanguage}</div>
                        <ul tabIndex={0}
                            className="dropdown-content menu bg-base-100 rounded-box z-[1] w-52 p-2 shadow">
                            <li><a onClick={() => setSelectedLanguage('Python')}>Python</a></li>
                            <li><a onClick={() => setSelectedLanguage('C++')}>C++</a></li>
                            <li><a onClick={() => setSelectedLanguage('C#')}>C#</a></li>
                        </ul>
                    </div>
                    <button className="btn ml-2">Submit</button>
                </div>
            </div>

            <div className="h-[1100px] w-1/2 flex flex-col items-center">
                <h1 className="h-[550] text-2xl font-bold mb-2">Title Here</h1>
                <p className="mb-4">Description Here</p>

                <div className="container mx-auto mt-auto p-4">
                    <div className="chat-container h-96 overflow-y-auto mb-4 p-4 bg-base-200 rounded-box">
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