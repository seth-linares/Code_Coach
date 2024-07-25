// src/components/TwoFactorSection.tsx

import React from 'react';
import { TwoFactorSectionProps } from '@/types';
import VerificationModal from "@/components/profile/VerificationModal";
import useTwoFactor from '@/hooks/useTwoFactorUI';

const TwoFactorSection: React.FC<TwoFactorSectionProps> = (props) => {
    const {
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
    } = useTwoFactor(props);

    return (
        <div className="card bg-base-100 shadow-xl mt-4">
            <div className="card-body">
                <h2 className="card-title">Two-Factor Authentication</h2>
                {props.status && (
                    <p>2FA is currently {props.status.is2faEnabled ? 'enabled' : 'disabled'}.</p>
                )}
                {message && <div className="alert alert-success mt-2">{message}</div>}
                {localError && !isModalOpen && <div className="alert alert-error mt-2">{localError}</div>}
                {props.status && !props.status.is2faEnabled ? (
                    <button className="btn btn-primary mt-2" onClick={handleEnable2FA} disabled={loading}>
                        Enable 2FA
                    </button>
                ) : (
                    props.status && props.status.is2faEnabled && (
                        <button className="btn btn-warning mt-2" onClick={handleDisable2FA} disabled={loading}>
                            Disable 2FA
                        </button>
                    )
                )}
                <VerificationModal
                    isOpen={isModalOpen}
                    onClose={() => setIsModalOpen(false)}
                    onVerify={handleVerify2FA}
                    verificationCode={verificationCode}
                    setVerificationCode={setVerificationCode}
                    loading={loading}
                    error={localError}
                />
            </div>
        </div>
    );
};

export default TwoFactorSection;