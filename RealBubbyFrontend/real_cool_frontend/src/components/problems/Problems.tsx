"use client"

import {useEffect, useRef, useState} from "react";
import * as monaco from 'monaco-editor';

const languageMap : {[key: string]: string} = { 'Python': 'python', 'C++': 'cpp', 'C#': 'csharp' };


export function Problems() {
    let monaco: any;
    if (typeof window !== 'undefined') {
        monaco = require('monaco-editor');
    }

    const editorRef = useRef(null);
    const editorInstance = useRef<monaco.editor.IStandaloneCodeEditor | null>(null);
    const [selectedLanguage, setSelectedLanguage] = useState("Python");


    useEffect(() => {
        if (editorRef.current && !editorInstance.current  && monaco) {
            
            editorInstance.current = monaco.editor.create(editorRef.current, {
                value: '',
                language: languageMap[selectedLanguage], // should be either python, cpp, or csharp
                theme: 'vs-dark'
            });
        }
    }, [selectedLanguage]);

    useEffect(() => {
        if (editorInstance.current) {
            const model = editorInstance.current.getModel();
            if (model) {
                monaco.editor.setModelLanguage(model, languageMap[selectedLanguage]);
            }
        }
    }, [selectedLanguage]);

    return (
        <div className="flex-grow bg-base-300 flex flex-row items-center justify-center py-10 pl-10">
            <div className="h-[1100px] w-1/2 flex flex-col items-center">
                <div ref={editorRef} className="h-full w-full mb-4">
                </div>
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