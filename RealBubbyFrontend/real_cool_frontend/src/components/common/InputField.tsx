// src/components/common/InputField.tsx

import React from 'react';

interface InputFieldProps {
    label: string;
    type: string;
    name: string;
    value: string;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    error?: string;
    required?: boolean;
}

const InputField: React.FC<InputFieldProps> = ({ label, type, name, value, onChange, error, required = false }) => {
    return (
        <div className="form-control mb-4">
            <label className="label">
                <span className="label-text">{label}</span>
            </label>
            <input
                type={type}
                name={name}
                className="input input-bordered"
                required={required}
                onChange={onChange}
                value={value}
            />
            {error && (
                <div className="alert alert-error mt-2">
                    {error}
                </div>
            )}
        </div>
    );
};

export default InputField;
