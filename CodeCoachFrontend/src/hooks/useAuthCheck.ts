// hooks/useAuthCheck.ts

import { useEffect, useState, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import axios from 'axios';

interface AuthCheckResult {
    isAuthenticated: boolean;
    isAuthChecking: boolean;
}

const PUBLIC_ROUTES = ['/', '/login', '/join', '/help', '/resources'];

export function useAuthCheck() {
    const [authState, setAuthState] = useState<AuthCheckResult>({ isAuthenticated: false, isAuthChecking: true });
    const router = useRouter();

    const checkAuth = useCallback(async () => {
        try {
            const response = await axios.get('https://www.codecoachapp.com/api/Users/CheckSession');
            const isAuthenticated = response.data.isAuthenticated;
            setAuthState({ isAuthenticated, isAuthChecking: false });
            return isAuthenticated;
        } catch (error) {
            console.error('Error checking authentication:', error);
            setAuthState({ isAuthenticated: false, isAuthChecking: false });
            return false;
        }
    }, []);

    useEffect(() => {
        checkAuth();
    }, [checkAuth]);

    const redirectBasedOnAuth = useCallback((isAuthenticated: boolean) => {
        const currentPath = window.location.pathname;

        if (isAuthenticated) {
            if (['/', '/login', '/join'].includes(currentPath)) {
                router.push('/dashboard');
            }
        } else {
            if (!PUBLIC_ROUTES.includes(currentPath)) {
                router.push('/login');
            }
        }
    }, [router]);

    return {
        ...authState,
        checkAuth,
        redirectBasedOnAuth
    };
}