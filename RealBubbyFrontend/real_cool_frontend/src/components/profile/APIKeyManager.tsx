// src/components/profile/APIKeyManager.tsx

import React, { useState, useCallback, useEffect } from 'react';
import useAPIKeys from '@/hooks/useAPIKeys';
import APIKeyTable from './APIKeyTable';
import APIKeyModal from './APIKeyModal';
import { APIKey, CreateAPIKeyRequest, UpdateAPIKeyRequest } from '@/types';

const APIKeyManager: React.FC = () => {
    const { apiKeys, loading, error, createAPIKey, deleteAPIKey, setActiveAPIKey, updateAPIKey, refreshAPIKeys } = useAPIKeys();
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [newKeyName, setNewKeyName] = useState('');
    const [newKeyValue, setNewKeyValue] = useState('');
    const [editingKey, setEditingKey] = useState<APIKey | null>(null);

    // Log apiKeys whenever it changes
    useEffect(() => {
        console.log('APIKeys in APIKeyManager:', apiKeys);
    }, [apiKeys]);

    const handleCreateKey = useCallback(async () => {
        console.log('Creating new key:', { KeyName: newKeyName, KeyValue: newKeyValue });
        const request: CreateAPIKeyRequest = { KeyName: newKeyName, KeyValue: newKeyValue };
        await createAPIKey(request);
        setNewKeyName('');
        setNewKeyValue('');
        setIsCreateModalOpen(false);
        refreshAPIKeys();
    }, [newKeyName, newKeyValue, createAPIKey, refreshAPIKeys]);

    const handleUpdateKey = useCallback(async () => {
        if (editingKey) {
            console.log('Updating key:', { ...editingKey, KeyValue: newKeyValue });
            const request: UpdateAPIKeyRequest = {
                APIKeyID: editingKey.apiKeyID,
                KeyName: editingKey.keyName,
                KeyValue: newKeyValue
            };
            await updateAPIKey(request);
            setEditingKey(null);
            setNewKeyValue('');
            setIsEditModalOpen(false);
            refreshAPIKeys();
        }
    }, [editingKey, newKeyValue, updateAPIKey, refreshAPIKeys]);

    const handleSetActive = useCallback(async (id: number) => {
        console.log('Setting active key:', id);
        await setActiveAPIKey(id);
        refreshAPIKeys();
    }, [setActiveAPIKey, refreshAPIKeys]);

    const handleDelete = useCallback(async (id: number) => {
        console.log('Deleting key:', id);
        await deleteAPIKey(id);
        refreshAPIKeys();
    }, [deleteAPIKey, refreshAPIKeys]);

    const handleEdit = useCallback((key: APIKey) => {
        console.log('Editing key:', key);
        setEditingKey(key);
        setNewKeyValue(''); // Clear the key value for security
        setIsEditModalOpen(true);
    }, []);

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
                    <button className="btn btn-primary" onClick={() => setIsCreateModalOpen(true)}>Create New Key</button>
                </div>
            </div>

            <APIKeyModal
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
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
                onClose={() => setIsEditModalOpen(false)}
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
