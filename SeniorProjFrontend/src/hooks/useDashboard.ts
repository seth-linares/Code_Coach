// src/hooks/useDashboard.ts

import { useState, useEffect, useCallback } from "react";
import axios from "axios";
import { GetProblemsRequest, GetProblemsResponse } from "@/types";

const API_URL = "https://www.codecoachapp.com/api/ProblemManagement/GetProblems";

interface ErrorResponse {
    status: number;
    message: string;
}

const useDashboard = () => {
    const [problems, setProblems] = useState<GetProblemsResponse[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<ErrorResponse | null>(null);
    const [filters, setFilters] = useState<GetProblemsRequest>({
        difficulty: "",
        category: "",
    });

    const fetchProblems = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const response = await axios.post<GetProblemsResponse[]>(API_URL, filters, {
                withCredentials: true, // This is important for sending cookies if you're using session-based auth
            });

            setProblems(response.data);
        } catch (err) {
            if (axios.isAxiosError(err)) {
                if (err.response) {
                    // The request was made and the server responded with a status code
                    // that falls out of the range of 2xx
                    setError({
                        status: err.response.status,
                        message: err.response.data || "An error occurred",
                    });
                } else if (err.request) {
                    // The request was made but no response was received
                    setError({
                        status: 0,
                        message: "No response received from server",
                    });
                } else {
                    // Something happened in setting up the request that triggered an Error
                    setError({
                        status: 0,
                        message: err.message || "An unknown error occurred",
                    });
                }
            } else {
                setError({
                    status: 0,
                    message: "An unknown error occurred",
                });
            }
        } finally {
            setLoading(false);
        }
    }, [filters]);

    useEffect(() => {
        fetchProblems();
    }, [fetchProblems]);

    const updateFilters = (newFilters: Partial<GetProblemsRequest>) => {
        setFilters((prev) => ({ ...prev, ...newFilters }));
    };

    const sortProblems = (key: keyof GetProblemsResponse) => {
        setProblems((prev) =>
            [...prev].sort((a, b) => {
                if (a[key] < b[key]) return -1;
                if (a[key] > b[key]) return 1;
                return 0;
            })
        );
    };

    return {
        problems,
        loading,
        error,
        filters,
        updateFilters,
        sortProblems,
        refreshProblems: fetchProblems,
    };
};

export default useDashboard;