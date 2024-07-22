// src/components/common/Modal.tsx

import React from 'react';
import {ModalProps} from "@/types";



const Modal: React.FC<ModalProps> = ({ isOpen, onClose, title, children }) => {
    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
            <div className="bg-white p-4 rounded shadow-lg w-1/3">
                <div className="flex justify-between items-center mb-4">
                    <h2 className="text-xl font-bold">{title}</h2>
                    <button onClick={onClose} className="text-xl font-bold">×</button>
                </div>
                {children}
            </div>
        </div>
    );
};

export default Modal;
