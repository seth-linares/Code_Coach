// src/components/profile/APIKeyModal.tsx

import React from 'react';
import { APIKeyModalProps } from "@/types";

const APIKeyModal: React.FC<APIKeyModalProps> = React.memo(({
    isOpen,
    onClose,
    title,
    keyName,
    keyValue,
    onKeyNameChange,
    onKeyValueChange,
    onSubmit,
    submitText
}) => {


    const handleKeyNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        onKeyNameChange(e.target.value);
    };

    const handleKeyValueChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        onKeyValueChange(e.target.value);
    };

    return (
        <div className={`modal ${isOpen ? 'modal-open' : ''}`}>
            <div className="modal-box">
                <h3 className="font-bold text-lg">{title}</h3>
                <input
                    type="text"
                    placeholder="Key Name"
                    className="input input-bordered w-full mt-4"
                    value={keyName}
                    onChange={handleKeyNameChange}
                />
                <input
                    type="text"
                    placeholder="Key Value"
                    className="input input-bordered w-full mt-2"
                    value={keyValue}
                    onChange={handleKeyValueChange}
                />
                <div className="modal-action">
                    <button className="btn" onClick={onClose}>Cancel</button>
                    <button className="btn btn-primary" onClick={onSubmit}>{submitText}</button>
                </div>
            </div>
        </div>
    );
});

APIKeyModal.displayName = 'Modal';

export default APIKeyModal;
