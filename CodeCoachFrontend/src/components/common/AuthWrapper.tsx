// components/AuthWrapper.tsx
"use client";

import React, { ReactNode, useEffect } from 'react';
import { useAuthCheck }  from '@/hooks/useAuthCheck';
import LoadingSpinner from './LoadingSpinner';

interface AuthWrapperProps {
    children: ReactNode;
    requireAuth?: boolean;
}

const AuthWrapper: React.FC<AuthWrapperProps> = React.memo(({ children, requireAuth = false }) => {
    const { isAuthenticated, isAuthChecking, checkAuth, redirectBasedOnAuth } = useAuthCheck();

    useEffect(() => {
        if (!isAuthChecking) {
            redirectBasedOnAuth(isAuthenticated);
        }
    }, [isAuthChecking, isAuthenticated, redirectBasedOnAuth]);

    if (isAuthChecking) {
        return <LoadingSpinner text="Checking authentication status..." />;
    }

    if (requireAuth && !isAuthenticated) {
        return <LoadingSpinner text="Redirecting to login..." />;
    }

    if (!requireAuth && isAuthenticated) {
        return <LoadingSpinner text="Redirecting..." />;
    }

    return <>{children}</>;
});

AuthWrapper.displayName = 'AuthWrapper';

export default AuthWrapper;