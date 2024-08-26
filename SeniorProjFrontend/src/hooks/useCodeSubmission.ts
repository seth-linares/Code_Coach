// src/hooks/useCodeSubmission.ts

import { useEffect, useState } from 'react';
import axios from 'axios';
import {SubmissionError, SubmissionResult, ProblemLanguageDetails, EditorState} from "@/types";

const API_URL = 'https://www.codecoachapp.com/api/UserSubmissions/SubmitCode';


const useCodeSubmission = (problemId: number, languageDetails: ProblemLanguageDetails[], editorState: EditorState) => {
    const [submitting, setSubmitting] = useState<boolean>(false);
    const [result, setResult] = useState<SubmissionResult | null>(null);
    const [error, setError] = useState<SubmissionError | null>(null);
    const [activeTab, setActiveTab] = useState<"editor" | "output">("editor");

    useEffect(() => {
        if(result !== null || error !== null) {
            setActiveTab("output");
        }
    }, [result, error]);

    const submitCode = async () => {
        const judge0LanguageId = getJudge0LanguageId();
        if (judge0LanguageId === null) {
            setError({ message: "No active language selected", status: 400 });
            return;
        }

        setSubmitting(true);
        setResult(null);
        setError(null);

        try {
            const response = await axios.post<SubmissionResult>(API_URL, {
                encodedCode: getEncodedActiveCode(),
                problemId: problemId,
                judge0LanguageId: judge0LanguageId
            });

            setResult(response.data);
        } catch (err) {
            if (axios.isAxiosError(err) && err.response) {
                setError({
                    message: err.response.data || "An error occurred while submitting the code",
                    status: err.response.status
                });
            } else {
                setError({
                    message: "An unexpected error occurred",
                    status: 500
                });
            }
        } finally {
            setSubmitting(false);
        }
    };

    const getJudge0LanguageId = (): number => {
        const activeLang: ProblemLanguageDetails | undefined = languageDetails.find(lang => lang.languageID === editorState.activeLanguage);
        return activeLang ? activeLang.judge0LanguageId : 51;
    };

    const getActiveCode = (): string => {
        return editorState.activeLanguage !== null
            ? editorState.codeByLanguage[editorState.activeLanguage]
            : '';
    };

    const getEncodedActiveCode = (): string => {
        return btoa(getActiveCode());
    };

    return {
        activeTab,
        setActiveTab,
        submitCode,
        submitting,
        result,
        error,
    };
};

export default useCodeSubmission;