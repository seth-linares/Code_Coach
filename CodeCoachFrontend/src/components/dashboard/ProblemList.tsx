// src/components/dashboard/ProblemList.tsx
"use client";

import React from "react";
import { GetProblemsResponse } from "@/types";
import ProblemCard from "./ProblemCard";

interface ProblemListProps {
    problems: GetProblemsResponse[];
    sortProblems: (key: keyof GetProblemsResponse) => void;
}

const ProblemList: React.FC<ProblemListProps> = ({ problems, sortProblems }) => {
    return (
        <div className="h-full">
            <div className="sticky top-0 bg-base-100 p-4 z-10 rounded-b-lg shadow-md">
                <div className="flex flex-wrap justify-center gap-2 max-w-3xl mx-auto">
                    <button className="btn btn-sm btn-outline" onClick={() => sortProblems("title")}>Sort by Title</button>
                    <button className="btn btn-sm btn-outline" onClick={() => sortProblems("difficulty")}>Sort by Difficulty</button>
                    <button className="btn btn-sm btn-outline" onClick={() => sortProblems("category")}>Sort by Category</button>
                    <button className="btn btn-sm btn-outline" onClick={() => sortProblems("points")}>Sort by Points</button>
                </div>
            </div>
            {problems.length > 0 ? (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 p-4">
                    {problems.map((problem) => (
                        <ProblemCard key={problem.problemID} problem={problem} />
                    ))}
                </div>
            ) : (
                <div className="text-center text-gray-500 mt-8">No problems found. Try adjusting your filters.</div>
            )}
        </div>
    );
};

export default ProblemList;