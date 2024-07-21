// src/components/profile/APIKeyTable.tsx

import React, { useEffect } from 'react';
import { APIKey, APIKeyTableProps } from '@/types';

const APIKeyTable: React.FC<APIKeyTableProps> = ({ apiKeys, onSetActive, onEdit, onDelete }) => {
    useEffect(() => {
        console.log('APIKeyTable rendered with keys:', JSON.stringify(apiKeys, null, 2));
    }, [apiKeys]);

    if (!Array.isArray(apiKeys)) {
        console.error('apiKeys is not an array:', apiKeys);
        return <div>Error: API keys data is invalid.</div>;
    }

    if (apiKeys.length === 0) {
        console.log('No API keys found');
        return <div>No API keys found.</div>;
    }

    const sortedApiKeys = [...apiKeys].sort((a, b) => (b.isActive ? 1 : 0) - (a.isActive ? 1 : 0));

    return (
        <div className="overflow-x-auto h-64">
            <table className="table w-full">
                <thead>
                <tr>
                    <th className="bg-base-200">Key Name</th>
                    <th className="bg-base-200">Usage Count</th>
                    <th className="bg-base-200">Status</th>
                    <th className="bg-base-200">Actions</th>
                </tr>
                </thead>
                <tbody>
                {sortedApiKeys.map((key, index) => {
                    console.log(`Rendering key ${index}:`, JSON.stringify(key, null, 2));
                    return (
                        <tr key={key.apiKeyID?.toString() || `fallback-key-${index}`} className="hover:bg-base-300">
                            <td className="bg-gray-700">{key.keyName || 'N/A'}</td>
                            <td className="bg-gray-700">{key.usageCount}</td>
                            <td className="bg-gray-700">
                                {key.isActive ? (
                                    <span className="badge badge-success">Active</span>
                                ) : (
                                    <span className="badge">Inactive</span>
                                )}
                            </td>
                            <td className="bg-gray-700">
                                <div className="btn-group space-x-2">
                                    <button
                                        className="btn btn-xs btn-primary"
                                        onClick={() => {
                                            console.log('Set Active clicked for key:', key.apiKeyID);
                                            onSetActive(key.apiKeyID);
                                        }}
                                    >
                                        Set Active
                                    </button>
                                    <button
                                        className="btn btn-xs btn-accent"
                                        onClick={() => {
                                            console.log('Edit clicked for key:', key);
                                            onEdit(key);
                                        }}
                                    >
                                        Edit
                                    </button>
                                    <button
                                        className="btn btn-xs btn-error"
                                        onClick={() => {
                                            console.log('Delete clicked for key:', key.apiKeyID);
                                            onDelete(key.apiKeyID);
                                        }}
                                    >
                                        Delete
                                    </button>
                                </div>
                            </td>
                        </tr>
                    );
                })}
                </tbody>
            </table>
        </div>
    );
};

export default APIKeyTable;
