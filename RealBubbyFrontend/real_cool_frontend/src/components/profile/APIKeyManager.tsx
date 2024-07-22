// src/components/profile/APIKeyManager.tsx
"use client";

import React, { useCallback } from 'react';
import APIKeyTable from './APIKeyTable';
import APIKeyModal from './APIKeyModal';
import useAPIKeyManager from "@/hooks/useAPIKeyManager";


const APIKeyManager: React.FC = () => {
    const {
        apiKeys,
        loading,
        error,
        isCreateModalOpen,
        isEditModalOpen,
        newKeyName,
        newKeyValue,
        editingKey,
        setIsCreateModalOpen,
        setIsEditModalOpen,
        setNewKeyName,
        setNewKeyValue,
        setEditingKey,
        handleCreateKey,
        handleUpdateKey,
        handleSetActive,
        handleDelete,
        handleEdit,
        refreshAPIKeys,
    } = useAPIKeyManager();

    const memoizedSetIsCreateModalOpen = useCallback(() => setIsCreateModalOpen(true), [setIsCreateModalOpen]);
    const memoizedSetIsEditModalOpenFalse = useCallback(() => setIsEditModalOpen(false), [setIsEditModalOpen]);
    const memoizedSetIsCreateModalOpenFalse = useCallback(() => setIsCreateModalOpen(false), [setIsCreateModalOpen]);

    return (
        <div className="card bg-base-100 shadow-xl mt-4">
            <div className="card-body">
                <h2 className="card-title mb-4">API Keys</h2>
                {error && (
                    <div className="alert alert-error shadow-lg mb-4">
                        <div className="flex items-center">
                            <span>{error}</span>
                            <button className="btn btn-sm ml-auto" onClick={refreshAPIKeys}>Retry</button>
                        </div>
                    </div>
                )}
                {loading ? (
                    <div className="flex justify-center items-center h-32">
                        <span className="loading loading-spinner loading-lg"></span>
                    </div>
                ) : (
                    <APIKeyTable
                        apiKeys={apiKeys}
                        onSetActive={handleSetActive}
                        onEdit={handleEdit}
                        onDelete={handleDelete}
                    />
                )}
                <div className="card-actions justify-end mt-4">
                    <button className="btn btn-primary" onClick={memoizedSetIsCreateModalOpen}>Create New Key</button>
                </div>
            </div>

            <APIKeyModal
                isOpen={isCreateModalOpen}
                onClose={memoizedSetIsCreateModalOpenFalse}
                title="Create New API Key"
                keyName={newKeyName}
                keyValue={newKeyValue}
                onKeyNameChange={setNewKeyName}
                onKeyValueChange={setNewKeyValue}
                onSubmit={handleCreateKey}
                submitText="Create"
            />

            <APIKeyModal
                isOpen={isEditModalOpen}
                onClose={memoizedSetIsEditModalOpenFalse}
                title="Edit API Key"
                keyName={editingKey?.keyName || ''}
                keyValue={newKeyValue}
                onKeyNameChange={(value) => setEditingKey(editingKey ? { ...editingKey, keyName: value } : null)}
                onKeyValueChange={setNewKeyValue}
                onSubmit={handleUpdateKey}
                submitText="Update"
            />
        </div>
    );
};

export default APIKeyManager;
