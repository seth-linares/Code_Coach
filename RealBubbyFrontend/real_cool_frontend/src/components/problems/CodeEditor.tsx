// src/components/problems/CodeEditor.tsx

"use client";

import React, { useEffect } from 'react';
import Editor, { OnMount } from "@monaco-editor/react";
import useCodeEditor from '@/hooks/useCodeEditor';
import useCodeSubmission from '@/hooks/useCodeSubmission';
import { ProblemLanguageDetails } from "@/types";
import { decodeBase64 } from "@/hooks/useProblemDetails";

interface CodeEditorProps {
    problemId: number;
    languageDetails: ProblemLanguageDetails[];
}

const languageMap: { [key: number]: string } = {
    51: 'csharp',  // C#
    54: 'cpp',     // C++
    92: 'python'   // Python
};

const CodeEditor: React.FC<CodeEditorProps> = ({ problemId, languageDetails }) => {
    const {
        activeLanguage,
        codeByLanguage,
        setActiveLanguage,
        updateCode,
        getActiveCode,
        getEncodedActiveCode,
    } = useCodeEditor(languageDetails);

    const getJudge0LanguageId = () => {
        const activeLang = languageDetails.find(lang => lang.languageID === activeLanguage);
        return activeLang ? activeLang.judge0LanguageId : null;
    };

    const {
        submitCode,
        submitting,
        result,
        error: submissionError
    } = useCodeSubmission(problemId, getEncodedActiveCode, getJudge0LanguageId);

    useEffect(() => {
        if (languageDetails.length > 0 && !activeLanguage) {
            setActiveLanguage(languageDetails[0].languageID);
        }
    }, [languageDetails, activeLanguage, setActiveLanguage]);

    const handleEditorDidMount: OnMount = (editor, monaco) => {
        // You can add any additional setup here if needed
    };

    const handleLanguageChange = (languageId: number) => {
        setActiveLanguage(languageId);
    };

    const handleEditorChange = (value: string | undefined) => {
        if (activeLanguage && value !== undefined) {
            updateCode(activeLanguage, value);
        }
    };

    const handleSubmit = () => {
        submitCode();
    };

    const getMonacoLanguage = () => {
        const activeLang = languageDetails.find(lang => lang.languageID === activeLanguage);
        return activeLang ? languageMap[activeLang.judge0LanguageId] : undefined;
    };

    const renderOutput = (label: string, content: string | undefined | null) => {
        if (!content) return null;
        const decodedContent = decodeBase64(content);
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
                    value={activeLanguage || ''}
                    onChange={(e) => handleLanguageChange(Number(e.target.value))}
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
                value={activeLanguage ? codeByLanguage[activeLanguage] : ''}
                theme="vs-dark"
                onChange={handleEditorChange}
                onMount={handleEditorDidMount}
                className="rounded-t-lg"
            />

            <div className="mt-2">
                <button
                    onClick={handleSubmit}
                    disabled={submitting}
                    className="btn btn-primary btn-sm"
                >
                    {submitting ? 'Submitting...' : 'Submit Code'}
                </button>
            </div>

            {result && (
                <div className="mt-4 p-4 bg-base-200 rounded-lg">
                    <h3 className="text-lg font-bold">Submission Result</h3>
                    <p>Status: {result.status.description}</p>
                    {result.executionTime !== undefined && <p>Execution Time: {result.executionTime}s</p>}
                    {result.memoryUsed !== undefined && <p>Memory Used: {result.memoryUsed} KB</p>}
                    {renderOutput("Standard Output", result.stdout)}
                    {renderOutput("Standard Error", result.stderr)}
                    {renderOutput("Compile Output", result.compileOutput)}
                </div>
            )}

            {submissionError && (
                <div className="mt-4 p-4 bg-error text-error-content rounded-lg">
                    <h3 className="text-lg font-bold">Submission Error</h3>
                    <p>{submissionError.message}</p>
                </div>
            )}
        </div>
    );
};

export default CodeEditor;