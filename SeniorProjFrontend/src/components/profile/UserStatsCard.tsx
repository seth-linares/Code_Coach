// src/components/profile/UserStatsCard.tsx

import React from 'react';
import Image from 'next/image';
import { UserStats } from '@/types';

interface UserStatsCardProps {
    stats: UserStats;
}

const UserStatsCard: React.FC<UserStatsCardProps> = ({ stats }) => {
    const completionRate = stats.attemptedProblems === 0
        ? "0%, No problems attempted yet"
        : `${((stats.completedProblems / stats.attemptedProblems) * 100).toFixed(1)}% completion rate`;

    return (
        <div className="card bg-base-100 shadow-xl">
            <div className="card-body">
                <div className="flex items-center gap-4 mb-6">
                    <div className="avatar">
                        <div className="w-24 rounded-full ring ring-primary ring-offset-base-100 ring-offset-2">
                            <Image
                                src={stats.profilePictureURL}
                                alt={`${stats.username}'s avatar`}
                                width={96}
                                height={96}
                                className="rounded-full"
                            />
                        </div>
                    </div>
                    <div>
                        <h2 className="text-2xl font-bold">{stats.username}</h2>
                        <p className="text-sm opacity-70">Registered: {new Date(stats.registrationDate).toLocaleDateString()}</p>
                    </div>
                </div>

                <div className="stats shadow">
                    <div className="stat">
                        <div className="stat-figure text-primary">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5}
                                 stroke="currentColor" className="w-8 h-8">
                                <path strokeLinecap="round" strokeLinejoin="round"
                                      d="M16.5 18.75h-9m9 0a3 3 0 013 3h-15a3 3 0 013-3m9 0v-3.375c0-.621-.503-1.125-1.125-1.125h-.871M7.5 18.75v-3.375c0-.621.504-1.125 1.125-1.125h.872m5.007 0H9.497m5.007 0a7.454 7.454 0 01-.982-3.172M9.497 14.25a7.454 7.454 0 00.981-3.172M5.25 4.236c-.982.143-1.954.317-2.916.52A6.003 6.003 0 007.73 9.728M5.25 4.236V4.5c0 2.108.966 3.99 2.48 5.228M5.25 4.236V2.721C7.456 2.41 9.71 2.25 12 2.25c2.291 0 4.545.16 6.75.47v1.516M7.73 9.728a6.726 6.726 0 002.748 1.35m8.272-6.842V4.5c0 2.108-.966 3.99-2.48 5.228m2.48-5.492a46.32 46.32 0 012.916.52 6.003 6.003 0 01-5.395 4.972m0 0a6.726 6.726 0 01-2.749 1.35m0 0a6.772 6.772 0 01-3.044 0"/>
                            </svg>
                        </div>
                        <div className="stat-title">Total Score</div>
                        <div className="stat-value text-primary">{stats.totalScore}</div>
                    </div>

                    <div className="stat">
                        <div className="stat-figure text-warning">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5}
                                 stroke="currentColor" className="w-8 h-8">
                                <path strokeLinecap="round" strokeLinejoin="round"
                                      d="M3.75 13.5l10.5-11.25L12 10.5h8.25L9.75 21.75 12 13.5H3.75z"/>
                            </svg>
                        </div>
                        <div className="stat-title">Rank</div>
                        <div className="stat-value text-warning">{stats.rank}</div>
                    </div>

                    <div className="stat">
                        <div className="stat-figure text-accent">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5}
                                 stroke="currentColor" className="w-8 h-8">
                                <path strokeLinecap="round" strokeLinejoin="round"
                                      d="M14.25 9.75L16.5 12l-2.25 2.25m-4.5 0L7.5 12l2.25-2.25M6 20.25h12A2.25 2.25 0 0020.25 18V6A2.25 2.25 0 0018 3.75H6A2.25 2.25 0 003.75 6v12A2.25 2.25 0 006 20.25z"/>
                            </svg>
                        </div>
                        <div className="stat-title">Completed Problems</div>
                        <div className="stat-value text-accent">{stats.completedProblems}</div>
                        <div className="stat-desc">Out of {stats.attemptedProblems} attempted</div>
                    </div>
                </div>

                <div className="mt-6">
                    <progress
                        className="progress progress-accent w-full"
                        value={stats.completedProblems}
                        max={stats.attemptedProblems || 1}
                    ></progress>
                    <p className="text-center mt-2">
                        {completionRate}
                    </p>
                </div>
            </div>
        </div>
    );
};

export default UserStatsCard;
