// src/components/dashboard/ProblemFilters.tsx

import React from "react";
import { GetProblemsRequest } from "@/types";

interface ProblemFiltersProps {
    filters: GetProblemsRequest;
    updateFilters: (newFilters: Partial<GetProblemsRequest>) => void;
}

const ProblemFilters: React.FC<ProblemFiltersProps> = ({ filters, updateFilters }) => {
    return (
        <div className="flex gap-4 mb-4">
            <select
                className="select select-bordered w-full max-w-xs"
                value={filters.difficulty}
                onChange={(e) => updateFilters({ difficulty: e.target.value })}
            >
                <option value="">All Difficulties</option>
                <option value="Easy">Easy</option>
                <option value="Medium">Medium</option>
                <option value="Hard">Hard</option>
            </select>
            <select
                className="select select-bordered w-full max-w-xs"
                value={filters.category}
                onChange={(e) => updateFilters({category: e.target.value})}
            >
                <option value="">All Categories</option>
                <option value="WarmUps">Warm Ups</option>
                <option value="Strings">Strings</option>
                <option value="Lists">Lists</option>
                {/* Add more categories as needed */}
            </select>
        </div>
    );
};

export default ProblemFilters;