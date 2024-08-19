// hooks/useAPIKeyManager.ts


import {useCallback, useEffect, useState} from "react";
import useAPIKeys from "@/hooks/useAPIKeys";
import {APIKey, CreateAPIKeyRequest, UpdateAPIKeyRequest} from "@/types";

const useAPIKeyManager = () => {
    const { apiKeys, loading, error, createAPIKey, deleteAPIKey, setActiveAPIKey, updateAPIKey, refreshAPIKeys } = useAPIKeys();
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [newKeyName, setNewKeyName] = useState('');
    const [newKeyValue, setNewKeyValue] = useState('');
    const [editingKey, setEditingKey] = useState<APIKey | null>(null);

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
        await refreshAPIKeys();
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
            await refreshAPIKeys();
        }
    }, [editingKey, newKeyValue, updateAPIKey, refreshAPIKeys]);

    const handleSetActive = useCallback(async (id: number) => {
        console.log('Setting active key:', id);
        await setActiveAPIKey(id);
        await refreshAPIKeys();
    }, [setActiveAPIKey, refreshAPIKeys]);

    const handleDelete = useCallback(async (id: number) => {
        console.log('Deleting key:', id);
        await deleteAPIKey(id);
        await refreshAPIKeys();
    }, [deleteAPIKey, refreshAPIKeys]);

    const handleEdit = useCallback((key: APIKey) => {
        console.log('Editing key:', key);
        setEditingKey(key);
        setNewKeyValue(''); // Clear the key value for security
        setIsEditModalOpen(true);
    }, []);

    return {
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
    };
}

export default useAPIKeyManager;