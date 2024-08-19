// src/hooks/useTwoFactor.ts

import { useState, useEffect } from 'react';
import { TwoFactorSectionProps } from '@/types';

const useTwoFactor = ({
                          status,
                          getStatus,
                          enable2FA,
                          verify2FA,
                          disable2FA,
                          loading: initialLoading,
                          error: initialError
                      }: TwoFactorSectionProps) => {
    const [verificationCode, setVerificationCode] = useState('');
    const [message, setMessage] = useState('');
    const [localError, setLocalError] = useState<string | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [loading, setLoading] = useState(initialLoading);

    useEffect(() => {
        getStatus();
    }, [getStatus]);

    useEffect(() => {
        setLocalError(initialError);
    }, [initialError]);

    const handleEnable2FA = async () => {
        setLocalError(null);
        setLoading(true);
        try {
            const result = await enable2FA();
            if (result) {
                setMessage(result);
                setIsModalOpen(true);
            }
        } catch (error) {
            setLocalError(error instanceof Error ? error.message : 'An error occurred');
        } finally {
            setLoading(false);
        }
    };

    const handleVerify2FA = async () => {
        setLocalError(null);
        setLoading(true);
        try {
            const result = await verify2FA(verificationCode);
            if (result) {
                setMessage(result);
                await getStatus();
                setIsModalOpen(false);
                setVerificationCode('');
            }
        } catch (error) {
            setLocalError(error instanceof Error ? error.message : 'An error occurred');
        } finally {
            setLoading(false);
        }
    };

    const handleDisable2FA = async () => {
        setLocalError(null);
        setLoading(true);
        try {
            const result = await disable2FA();
            if (result) {
                setMessage(result);
                await getStatus();
            }
        } catch (error) {
            setLocalError(error instanceof Error ? error.message : 'An error occurred');
        } finally {
            setLoading(false);
        }
    };

    return {
        verificationCode,
        setVerificationCode,
        message,
        localError,
        isModalOpen,
        setIsModalOpen,
        loading,
        handleEnable2FA,
        handleVerify2FA,
        handleDisable2FA
    };
};

export default useTwoFactor;