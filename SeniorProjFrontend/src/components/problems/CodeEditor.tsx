// src/components/problems/CodeEditor.tsx

"use client";

import React, { useEffect } from 'react';
import Editor from "@monaco-editor/react";
import useCodeEditor from '@/hooks/useCodeEditor';
import useCodeSubmission from '@/hooks/useCodeSubmission';
import { ProblemLanguageDetails, CodeEditorProps } from "@/types";
import { decodeBase64 } from "@/hooks/useProblemDetails";




const CodeEditor: React.FC<CodeEditorProps> = React.memo(({ problemId, languageDetails }) => {
    const {
        editorState,
        setActiveLanguage,
        updateCode,
        getMonacoLanguage,
    } = useCodeEditor(languageDetails);



    const {
        submitCode,
        submitting,
        result,
        error: submissionError, // create const with new name to be more clear
    } = useCodeSubmission(problemId, languageDetails, editorState);


    const printStuff = () => {
        console.log(editorState);
    };



    const renderOutput = (label: string, content: string | undefined | null) => {
        if (!content) return null;
        const decodedContent: string = decodeBase64(content);
        return (
            <div className="mt-2">
                <h4 className="font-bold">{label}:</h4>
                <pre className="whitespace-pre-wrap p-2 rounded">{decodedContent}</pre>
            </div>
        );
    };

    return (
        <div className="flex flex-col">
            <div className="mb-2">
                <select
                    value={editorState.activeLanguage || ''}
                    onChange={(e) => setActiveLanguage(Number(e.target.value))}
                    className="select select-bordered select-sm"
                >
                    {languageDetails.map((lang) => (
                        <option key={lang.languageID} value={lang.languageID}>
                            {lang.languageName}
                        </option>
                    ))}
                </select>
            </div>

            <Editor
                height="60vh"
                language={getMonacoLanguage()}
                value={editorState.activeLanguage ? editorState.codeByLanguage[editorState.activeLanguage] : ''}
                onChange={updateCode}
                theme="vs-dark"
                className="rounded-t-lg"
            />

            <div className="mt-2">
                <button
                    onClick={submitCode}
                    disabled={submitting}
                    className="btn btn-primary btn-sm"
                >
                    {submitting ? 'Submitting...' : 'Submit Code'}
                </button>
            </div>

            <div className="mt-2">
                <button
                    onClick={printStuff}
                    disabled={submitting}
                    className="btn btn-primary btn-sm"
                >
                    Test Button
                </button>
            </div>

            {result && (
                <div
                    className={`mt-4 p-4 rounded-lg ${result.isSuccessful ? 'bg-success text-success-content' : 'bg-error text-error-content'}`}>
                    <div className="flex items-center mb-2">
                        {result.isSuccessful ? (
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none"
                                 viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7"/>
                            </svg>
                        ) : (
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none"
                                 viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                                      d="M6 18L18 6M6 6l12 12"/>
                            </svg>
                        )}
                        <h3 className="text-lg font-bold">
                            {result.isSuccessful ? 'Submission Successful' : 'Submission Failed'}
                        </h3>
                    </div>
                    <p className="font-semibold">Status: {result.status.description}</p>
                    {result.executionTime !== undefined && <p>Execution Time: {result.executionTime}s</p>}
                    {result.memoryUsed !== undefined && <p>Memory Used: {result.memoryUsed} KB</p>}
                    {renderOutput("Standard Output", result.stdout)}
                    {renderOutput("Standard Error", result.stderr)}
                    {renderOutput("Compile Output", result.compileOutput)}
                </div>
            )}

            {submissionError && (
                <div className="mt-4 p-4 bg-error text-error-content rounded-lg">
                    <div className="flex items-center mb-2">
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24"
                             stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                                  d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
                        </svg>
                        <h3 className="text-lg font-bold">Submission Error</h3>
                    </div>
                    <p>{submissionError.message}</p>
                </div>
            )}
        </div>
    );
});

CodeEditor.displayName = 'CodeEditor';

export default CodeEditor;