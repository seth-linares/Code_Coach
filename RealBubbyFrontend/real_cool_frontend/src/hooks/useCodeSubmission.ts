// src/hooks/useCodeSubmission.ts

"use client";

import { useState } from 'react';
import axios from 'axios';
import { SubmissionError, SubmissionResult } from "@/types";

const API_URL = 'https://localhost/api/UserSubmissions/SubmitCode';

const useCodeSubmission = (problemId: number, getEncodedActiveCode: () => string, getJudge0LanguageId: () => number | null) => {
    const [submitting, setSubmitting] = useState(false);
    const [result, setResult] = useState<SubmissionResult | null>(null);
    const [error, setError] = useState<SubmissionError | null>(null);

    const submitCode = async () => {
        const judge0LanguageId = getJudge0LanguageId();
        console.log(`Judge0 Language ID: ${judge0LanguageId}`);
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

    return { submitCode, submitting, result, error };
};

export default useCodeSubmission;