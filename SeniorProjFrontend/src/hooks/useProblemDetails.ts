// src/hooks/useProblemDetails.ts

import { useState, useEffect } from 'react';
import axios from 'axios';
import { ProblemDetails, ProblemLanguageDetails } from "@/types";

const API_URL: string = "https://www.codecoachapp.com/api/ProblemManagement/GetProblemDetails";

export const decodeBase64 = (str: string): string => {
    try {
        const decodedString = atob(str);
        const bytes = new Uint8Array(decodedString.split('').map(char => char.charCodeAt(0)));
        const decoder = new TextDecoder('utf-8');
        return decoder.decode(bytes);
    } catch (e) {
        console.error('Failed to decode base64:', e);
        return 'Error decoding description.';
    }
};

const useProblemDetails = (problemId: number) => {
    const [problemDetails, setProblemDetails] = useState<ProblemDetails | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchProblemDetails = async () => {
            try {
                setLoading(true);
                const response = await axios.post(API_URL, { id: problemId });
                const decodedData: ProblemDetails = {
                    ...response.data,
                    description: decodeBase64(response.data.description),
                    languageDetails: response.data.languageDetails.map((lang: ProblemLanguageDetails) => ({
                        ...lang,
                        functionSignature: decodeBase64(lang.functionSignature)
                    }))
                };
                setProblemDetails(decodedData);
                setError(null);
            } catch (err) {
                setError('Failed to fetch problem details');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        fetchProblemDetails();
    }, [problemId]);

    return {
        problemDetails,
        loading,
        error
    };
};

export default useProblemDetails;
