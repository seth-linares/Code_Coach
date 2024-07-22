// src/components/common/InputField.tsx

import React from 'react';

interface InputFieldProps {
    label: string;
    type: string;
    name: string;
    value: string | boolean;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    error?: string;
    required?: boolean;
}

const InputField: React.FC<InputFieldProps> = React.memo(({ label, type, name, value, onChange, error, required = false }) => {
    if (type === 'checkbox') {
        return (
            <div className="form-control mb-4">
                <label className="label cursor-pointer justify-start gap-2">
                    <input
                        type="checkbox"
                        name={name}
                        className="checkbox"
                        required={required}
                        onChange={onChange}
                        checked={value as boolean}
                    />
                    <span className="label-text">{label}</span>
                </label>
                {error && (
                    <div className="alert alert-error mt-2">
                        {error}
                    </div>
                )}
            </div>
        );
    }

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
                value={value as string}
            />
            {error && (
                <div className="alert alert-error mt-2">
                    {error}
                </div>
            )}
        </div>
    );
});

InputField.displayName = 'InputField';

export default InputField;