// src/components/profile/VerificationModal.tsx

import React from "react";
import {VerificationModalProps} from "@/types";

const VerificationModal: React.FC<VerificationModalProps> = ({
                                                                 isOpen,
                                                                 onClose,
                                                                 onVerify,
                                                                 verificationCode,
                                                                 setVerificationCode,
                                                                 loading,
                                                                 error
                                                             }) => {

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-base-100 p-6 rounded-lg shadow-xl max-w-md w-full">
                <h3 className="text-xl font-bold mb-4 text-base-content">Enter Verification Code</h3>
                <p className="text-sm text-base-content/70 mb-4">
                    Please enter the verification code sent to your email.
                </p>
                <input
                    type="text"
                    placeholder="Enter verification code"
                    className="input input-bordered w-full mb-4"
                    value={verificationCode}
                    onChange={(e) => setVerificationCode(e.target.value)}
                    disabled={loading}
                />
                {error && <div className="alert alert-error mb-4">{error}</div>}
                <div className="flex justify-center space-x-4">
                    <button
                        className="btn btn-ghost"
                        onClick={onClose}
                        disabled={loading}
                    >
                        Cancel
                    </button>
                    <button
                        className="btn btn-primary"
                        onClick={onVerify}
                        disabled={loading}
                    >
                        {loading ? 'Verifying...' : 'Verify Code'}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default VerificationModal;