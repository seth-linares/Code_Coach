// components/common/LoadingSpinner.tsx

"use client";

import React from 'react';

interface LoadingSpinnerProps {
    size?: 'xs' | 'sm' | 'md' | 'lg';
    text?: string;
}

const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({ size = 'md', text = 'Loading...' }) => {
    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-base-300">
            <div className={`loading loading-spinner loading-${size}`}></div>
            {text && <p className="mt-4 text-base-content">{text}</p>}
        </div>
    );
};

export default LoadingSpinner;