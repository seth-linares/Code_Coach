// src/components/dashboard/DashboardPage.tsx
"use client";

import React from "react";
import useDashboard from "@/hooks/useDashboard";
import ProblemFilters from "./ProblemFilters";
import ProblemList from "./ProblemList";

const DashboardPage: React.FC = () => {
    const {
        problems,
        loading,
        error,
        filters,
        updateFilters,
        sortProblems
    } = useDashboard();

    if (loading) return <div className="flex justify-center items-center h-screen"><span className="loading loading-spinner loading-lg"></span></div>;

    return (
        <div className="container mx-auto p-4 h-screen flex flex-col">
            <h1 className="text-3xl font-bold mb-4">Problem Dashboard</h1>
            {error && (
                <div className="alert alert-error shadow-lg mb-4">
                    <div>
                        <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current flex-shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                        <span>
                            {error.status === 401 && "You are not authorized. Please log in."}
                            {error.status === 400 && "Invalid input. Please check your filters."}
                            {error.status >= 500 && "A server error occurred. Please try again later."}
                            {error.status === 0 && error.message}
                        </span>
                    </div>
                </div>
            )}
            <ProblemFilters filters={filters} updateFilters={updateFilters} />
            <div className="flex-grow overflow-hidden">
                <div className="h-full overflow-y-auto">
                    <ProblemList problems={problems} sortProblems={sortProblems} />
                </div>
            </div>
        </div>
    );
};

export default DashboardPage;