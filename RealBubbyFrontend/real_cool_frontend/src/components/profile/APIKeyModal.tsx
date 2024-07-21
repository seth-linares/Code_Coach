import React from 'react';
import { APIKeyModalProps } from "@/types";

const APIKeyModal: React.FC<APIKeyModalProps> = ({
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
    return (
        <div className={`modal ${isOpen ? 'modal-open' : ''}`}>
            <div className="modal-box">
                <h3 className="font-bold text-lg">{title}</h3>
                <input
                    type="text"
                    placeholder="Key Name"
                    className="input input-bordered w-full mt-4"
                    value={keyName}
                    onChange={(e) => onKeyNameChange(e.target.value)}
                />
                <input
                    type="text"
                    placeholder="Key Value"
                    className="input input-bordered w-full mt-2"
                    value={keyValue}
                    onChange={(e) => onKeyValueChange(e.target.value)}
                />
                <div className="modal-action">
                    <button className="btn" onClick={onClose}>Cancel</button>
                    <button className="btn btn-primary" onClick={onSubmit}>{submitText}</button>
                </div>
            </div>
        </div>
    );
};

export default APIKeyModal;