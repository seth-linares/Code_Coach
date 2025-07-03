// src/components/dashboard/ProblemCard.tsx
"use client";

import React from "react";
import { GetProblemsResponse } from "@/types";
import Link from "next/link";

interface ProblemCardProps {
    problem: GetProblemsResponse;
}

const ProblemCard: React.FC<ProblemCardProps> = ({ problem }) => {
    const difficultyColor = {
        Easy: 'bg-green-100 text-green-800',
        Medium: 'bg-yellow-100 text-yellow-800',
        Hard: 'bg-red-100 text-red-800'
    }[problem.difficulty] || 'bg-gray-100 text-gray-800';

    return (
        <Link href={`/problems/${problem.problemID}`}>
            <div className={`card bg-base-100 shadow-xl hover:shadow-2xl transition-shadow duration-300 overflow-hidden ${problem.isCompleted ? 'border-2 border-green-500' : ''}`}>
                <div className="card-body p-4">
                    <div className="flex justify-between items-center mb-2">
                        <h2 className="card-title text-lg font-semibold">{problem.title}</h2>
                        {problem.isCompleted && (
                            <div className="badge badge-success gap-2">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="inline-block w-4 h-4 stroke-current">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 13l4 4L19 7"></path>
                                </svg>
                                Completed
                            </div>
                        )}
                    </div>
                    <div className="flex flex-wrap gap-2 mt-2">
            <span className={`px-2 py-1 rounded-full text-xs font-medium ${difficultyColor}`}>
              {problem.difficulty}
            </span>
                        <span className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-medium">
              {problem.category}
            </span>
                        <span className="px-2 py-1 bg-purple-100 text-purple-800 rounded-full text-xs font-medium">
              {problem.points} points
            </span>
                    </div>
                </div>
            </div>
        </Link>
    );
};

export default ProblemCard;