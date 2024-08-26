// src/components/problems/CodeEditor.tsx

// ADD RESPONSIVE FONT SIZING

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
        fontSize,
        setFontSize,
    } = useCodeEditor(languageDetails);



    const {
        activeTab,
        setActiveTab,
        submitCode,
        submitting,
        result,
        error: submissionError, // create const with new name to be more clear
    } = useCodeSubmission(problemId, languageDetails, editorState);

    



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
        <div className="flex flex-col h-full z-2">
            <div className="tabs tabs-lifted">
                {/* EACH TAB IS 4 PX OFF IN SOME DIRECTION */}
                <a className={`tab ${activeTab === 'editor' ? 'tab-active font-semibold' : ''} right-1`} onClick={() => setActiveTab('editor')}>Code Editor</a> 
                <a className={`tab ${activeTab === 'output' ? 'tab-active font-semibold' : ''} left-1`} onClick={() => setActiveTab('output')}>Output</a>
            </div>
            <div className="flex-grow overflow-hidden">
                {activeTab === 'editor' && (
                    <div className="h-full flex flex-col p-4 bg-base-100 rounded-b">
                        <div className="mb-2">
                            <select
                                value={fontSize || ''}
                                onChange={(e) => setFontSize(Number(e.target.value))}
                                className="select select-bordered select-sm max-w-xs"
                            >
                                {[12, 14, 16, 18, 20, 22, 24, 36, 48, 60, 72, 84].map((size) => (
                                    <option key={size} value={size}>
                                        {size}
                                    </option>
                                ))}
                            </select>
                            <select
                                value={editorState.activeLanguage || ''}
                                onChange={(e) => setActiveLanguage(Number(e.target.value))}
                                className="ml-2 select select-bordered select-sm max-w-xs"
                            >
                                {languageDetails.map((lang) => (
                                    <option key={lang.languageID} value={lang.languageID}>
                                        {lang.languageName}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="flex-grow">
                            <Editor
                                height="100%"
                                language={getMonacoLanguage()}
                                value={editorState.activeLanguage ? editorState.codeByLanguage[editorState.activeLanguage] : ''}
                                onChange={updateCode}
                                options={{
                                    fontSize: fontSize,
                                }}
                                theme="vs-dark"
                                className="rounded-t-lg"
                            />
                        </div>
                        <div className="mt-2">
                            <button
                                onClick={submitCode}
                                disabled={submitting}
                                className="btn btn-primary btn-sm w-full"
                            >
                                {submitting ? 'Submitting...' : 'Submit Code'}
                            </button>
                        </div>
                    </div>
                )}

                {activeTab === 'output' && (result !== null || submissionError !== null) ? (
                    <div className="h-full overflow-auto bg-base-100">
                        {result && (
                            <div className={`mt-4 p-4 rounded-b ${result.isSuccessful ? 'bg-success text-success-content' : 'bg-error text-error-content'}`}>
                                <div className="flex items-center mb-2">
                                    {result.isSuccessful ? (
                                        <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7"/>
                                        </svg>
                                    ) : (
                                        <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12"/>
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
                            <div className="mt-4 p-4 bg-error text-error-content rounded-b">
                                <div className="flex items-center mb-2">
                                    <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
                                    </svg>
                                    <h3 className="text-lg font-bold">Submission Error</h3>
                                </div>
                                <p>{submissionError.message}</p>
                            </div>
                        )}
                    </div>
                ) : (<div className='bg-base-100 h-full rounded-b'> <p className='italic p-4'>This is where the output will be displayed...</p></div>)}
            </div>
        </div>
    );
});

CodeEditor.displayName = 'CodeEditor';

export default CodeEditor;