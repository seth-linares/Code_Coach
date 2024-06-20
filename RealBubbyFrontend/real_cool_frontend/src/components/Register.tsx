// pages/Register.tsx
"use client"

import React, { useState } from 'react';
import axios from 'axios';
import Link from 'next/link';
import {RegisterRequestBody as FormData} from "@/app/api/register"


export function Register() {
    const [formData, setFormData] = useState<FormData>({
        username: '',
        password: '',
        comparePassword: '',
        emailAddress: ''
    });
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);
        setSuccess(null);

        // Basic client-side validation
        if (formData.password !== formData.comparePassword) {
            setError("Passwords do not match");
            return;
        }

        try {
            // POST request to the Next.js API route
            const response = await axios.post('/api/register', formData);
            setSuccess('Registration successful!');
            setFormData({ username: '', password: '', comparePassword: '', emailAddress: '' });
        } catch (error: any) {
            setError(error.response?.data?.message || 'An error occurred');
        }
    };

    return (
        <div className="container mx-auto px-4">
            <form onSubmit={handleSubmit} className="form bg-base-200 p-6 my-10 rounded-lg shadow-lg max-w-md mx-auto">
                <h2 className="text-2xl font-bold mb-6 text-center">Register</h2>

                {error && <div className="alert alert-error mb-4">{error}</div>}
                {success && <div className="alert alert-success mb-4">{success}</div>}

                <div className="form-control mb-4">
                    <label className="label">
                        <span className="label-text">Username</span>
                    </label>
                    <input type="text" name="username" className="input input-bordered" required onChange={handleChange} value={formData.username} />
                </div>

                <div className="form-control mb-4">
                    <label className="label">
                        <span className="label-text">Email</span>
                    </label>
                    <input type="email" name="emailAddress" className="input input-bordered" required onChange={handleChange} value={formData.emailAddress} />
                </div>

                <div className="form-control mb-4">
                    <label className="label">
                        <span className="label-text">Password</span>
                    </label>
                    <input type="password" name="password" className="input input-bordered" required onChange={handleChange} value={formData.password} />
                </div>

                <div className="form-control mb-4">
                    <label className="label">
                        <span className="label-text">Confirm Password</span>
                    </label>
                    <input type="password" name="comparePassword" className="input input-bordered" required onChange={handleChange} value={formData.comparePassword} />
                </div>

                <div className="form-control mt-6">
                    <button type="submit" className="btn btn-primary">Register</button>
                </div>
            </form>

            <p className="text-center">Already have an account? <Link href={"/login"} className="link link-primary">Login</Link></p>
        </div>
    );
}
