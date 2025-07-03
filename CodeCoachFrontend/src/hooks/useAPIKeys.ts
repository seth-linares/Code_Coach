// hooks/useAPIKeys.ts

import { useState, useEffect, useCallback } from 'react';
import axios, { AxiosError } from 'axios';
import { APIKey, CreateAPIKeyRequest, UpdateAPIKeyRequest, ErrorResponse } from "@/types";



const useAPIKeys = () => {
    const [apiKeys, setApiKeys] = useState<APIKey[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleError = (err: unknown, defaultMessage: string) => {
        if (axios.isAxiosError(err)) {
            const axiosError = err as AxiosError<ErrorResponse>;
            if (axiosError.response?.status === 404) {
                setError('No API keys found or unauthorized access');
            } else {
                const errorMessage = axiosError.response?.data?.message || axiosError.message || defaultMessage;
                setError(errorMessage);
            }
        } else {
            setError(defaultMessage);
        }
        console.error(err);
    };

    const fetchAPIKeys = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await axios.get<APIKey[]>('https://www.codecoachapp.com/api/APIKeys/ListAPIKeys');
            console.log('\n\nRaw response data:', response.data);
            console.log('\n\nParsed keys:');
            response.data.forEach((key, index) => {
                console.log(`Key ${index}:`, JSON.stringify(key, null, 2));
            });
            setApiKeys(response.data);
        } catch (err) {
            handleError(err, 'Failed to fetch API keys');
        } finally {
            setLoading(false);
        }
    }, []);

    const createAPIKey = async (request: CreateAPIKeyRequest) => {
        setLoading(true);
        setError(null);
        try {
            const response = await axios.post<{ Message: string, APIKeyID: number }>('https://www.codecoachapp.com/api/APIKeys/CreateAPIKey', request);
            const newKey: APIKey = {
                apiKeyID: response.data.APIKeyID,
                keyName: request.KeyName,
                isActive: true,
                createdAt: new Date().toISOString(),
                lastUsedAt: null,
                usageCount: 0,
            };
            setApiKeys(prevKeys => [newKey, ...prevKeys.map(key => ({ ...key, IsActive: false }))]);
        } catch (err) {
            handleError(err, 'Failed to create API key');
        } finally {
            setLoading(false);
        }
    };

    const deleteAPIKey = async (keyId: number) => {
        setLoading(true);
        setError(null);
        try {
            await axios.delete('https://www.codecoachapp.com/api/APIKeys/DeleteAPIKey', { data: { Id: keyId } });
            setApiKeys(prevKeys => prevKeys.filter(key => key.apiKeyID !== keyId));
        } catch (err) {
            handleError(err, 'Failed to delete API key');
            await fetchAPIKeys(); // Refetch in case of error
        } finally {
            setLoading(false);
        }
    };

    const setActiveAPIKey = async (keyId: number) => {
        setLoading(true);
        setError(null);
        try {
            await axios.put('https://www.codecoachapp.com/api/APIKeys/SetActiveAPIKey', { Id: keyId });
            setApiKeys(prevKeys => prevKeys.map(key => ({
                ...key,
                IsActive: key.apiKeyID === keyId
            })));
        } catch (err) {
            handleError(err, 'Failed to set active API key');
            await fetchAPIKeys(); // Refetch in case of error
        } finally {
            setLoading(false);
        }
    };

    const updateAPIKey = async (request: UpdateAPIKeyRequest) => {
        setLoading(true);
        setError(null);
        try {
            await axios.put('https://www.codecoachapp.com/api/APIKeys/UpdateAPIKey', request);
            setApiKeys(prevKeys => prevKeys.map(key =>
                key.apiKeyID === request.APIKeyID
                    ? { ...key, KeyName: request.KeyName }
                    : key
            ));
        } catch (err) {
            handleError(err, 'Failed to update API key');
            await fetchAPIKeys(); // Refetch in case of error
        } finally {
            setLoading(false);
        }
    };

    const getAPIKeyById = (keyId: number) => {
        return apiKeys.find(key => key.apiKeyID === keyId);
    };

    useEffect(() => {
        fetchAPIKeys();
    }, [fetchAPIKeys]);

    return {
        apiKeys,
        loading,
        error,
        createAPIKey,
        deleteAPIKey,
        setActiveAPIKey,
        updateAPIKey,
        refreshAPIKeys: fetchAPIKeys,
    };
};

export default useAPIKeys;