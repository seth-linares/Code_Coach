// src/components/profile/ProfileHeader.tsx
"use client";

import React, { useState } from 'react';
import InputField from '../common/InputField';
import Link from "next/link";
import { useProfileActions } from '@/hooks/useProfileActions';

const ProfileHeader: React.FC = () => {
    const { loading, error, message, validationErrors, changePassword, logout } = useProfileActions();
    const [isChangePasswordModalOpen, setIsChangePasswordModalOpen] = useState(false);
    const [passwordData, setPasswordData] = useState({
        currentPassword: '',
        newPassword: '',
        confirmNewPassword: '',
    });

    const handleLogout = async () => {
        const success = await logout();
        if (!success) {
            // Handle logout failure (error message is already set in the hook)
            console.error('Logout failed');
        }
    };

    const handleOpenChangePasswordModal = () => {
        setIsChangePasswordModalOpen(true);
    };

    const handleCloseChangePasswordModal = () => {
        setIsChangePasswordModalOpen(false);
        setPasswordData({
            currentPassword: '',
            newPassword: '',
            confirmNewPassword: '',
        });
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setPasswordData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleChangePassword = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const success = await changePassword(passwordData);
        if (success) {
            handleCloseChangePasswordModal();
        }
    };

    return (
        <div className="card flex justify-between items-center p-4 bg-base-100 shadow-md mb-4">
            <h1 className="text-2xl font-bold mb-2">Profile and Settings</h1>
            <div className="flex space-x-4">
                <Link href={"/dashboard"} className="btn btn-secondary">
                    Dashboard
                </Link>
                <button className="btn btn-info" onClick={handleOpenChangePasswordModal}>
                    Change Password
                </button>
                <button className="btn btn-error" onClick={handleLogout} disabled={loading}>
                    {loading ? 'Logging out...' : 'Logout'}
                </button>
            </div>
            {error && <div className="alert alert-error">{error}</div>}
            {message && <div className="alert alert-success">{message}</div>}
            <div className={`modal ${isChangePasswordModalOpen ? 'modal-open' : ''}`}>
                <div className="modal-box">
                    <h3 className="font-bold text-lg">Change Password</h3>
                    {error && <div className="alert alert-error mb-4">{error}</div>}
                    {message && <div className="alert alert-success mb-4">{message}</div>}
                    <form onSubmit={handleChangePassword}>
                        <InputField
                            label="Old Password"
                            type="password"
                            name="currentPassword"
                            value={passwordData.currentPassword}
                            onChange={handleInputChange}
                            error={validationErrors?.CurrentPassword?.join(', ')}
                            required
                        />
                        <InputField
                            label="New Password"
                            type="password"
                            name="newPassword"
                            value={passwordData.newPassword}
                            onChange={handleInputChange}
                            error={validationErrors?.NewPassword?.join(', ')}
                            required
                        />
                        <InputField
                            label="Confirm New Password"
                            type="password"
                            name="confirmNewPassword"
                            value={passwordData.confirmNewPassword}
                            onChange={handleInputChange}
                            error={validationErrors?.ConfirmNewPassword?.join(', ')}
                            required
                        />
                        <div className="modal-action">
                            <button type="button" className="btn" onClick={handleCloseChangePasswordModal}>
                                Cancel
                            </button>
                            <button type="submit" className="btn btn-primary" disabled={loading}>
                                {loading ? 'Changing...' : 'Change Password'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default ProfileHeader;