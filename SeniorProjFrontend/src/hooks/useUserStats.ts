// src/hooks/useUserStats.ts


import { useState, useEffect } from 'react';
import axios from 'axios';
import { UserStats } from '@/types';

const useUserStats = () => {
    const [stats, setStats] = useState<UserStats | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchStats = async () => {
            try {
                const response = await axios.get<UserStats>('https://www.codecoachapp.com/api/Users/stats', {
                    withCredentials: true,
                });
                setStats(response.data);
            } catch (err) {
                if (axios.isAxiosError(err)) {
                    setError(err.response?.data?.title || err.message || 'Failed to fetch user stats');
                } else {
                    setError('An unexpected error occurred');
                }
            } finally {
                setLoading(false);
            }
        };

        fetchStats();
    }, []);

    return { stats, loading, error };
};

export default useUserStats;
