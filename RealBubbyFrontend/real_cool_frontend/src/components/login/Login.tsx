// src/components/login/Login.tsx
"use client"

import React, { useState } from 'react';
import axios from 'axios';
import Link from 'next/link';
import InputField from "@/components/common/InputField";
import { LoginRequestBody as FormData, LoginResponse, UnauthorizedError } from "@/pages/api/login";
import { useRouter, usePathname, useSearchParams } from 'next/navigation'



export function Login() {
    const [formData, setFormData] = useState<FormData>({
            username: '',
            password: ''
    });

    const [error, setError] = useState<string | null>(null); // General errors
    const [errors, setErrors] = useState<UnauthorizedError>({}); // Auth errors

    const router = useRouter()
    const pathname = usePathname()
    const searchParams = useSearchParams()

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);
        setErrors({});

        try {
            const response = await axios.post<LoginResponse>('/api/login', formData);
            const { token }: LoginResponse = response.data;
            // Handle success (e.g., storing the JWT token, redirecting the user)
            console.log(token); // You can replace this with your own logic
            router.push('/dashboard'); // Redirect to the home page after successful login
        } catch (error: any) {
            setError(error.response?.data?.message || 'An error occurred');
        }
    };

    return (
        <div className="container mx-auto px-4">
            <form onSubmit={handleSubmit} className="form bg-base-200 p-6 my-10 rounded-lg shadow-lg max-w-md mx-auto">
                <h2 className="text-2xl font-bold mb-6 text-center">Login</h2>

                {error && <div className="alert alert-error mb-4">{error}</div>}

                <div className="form-control mb-4">
                    <label className="label">
                        <span className="label-text">Username</span>
                    </label>
                    <input type="text" name="username" className="input input-bordered" required onChange={handleChange} value={formData.username} />
                </div>

                <div className="form-control mb-4">
                    <label className="label">
                        <span className="label-text">Password</span>
                    </label>
                    <input type="password" name="password" className="input input-bordered" required onChange={handleChange} value={formData.password} />
                </div>

                <div className="form-control mt-6">
                    <button type="submit" className="btn btn-primary">Login</button>
                </div>
            </form>



            <p className="text-center">Don&apos;t have an account?  {/* "&apos;" is used to replace "'" because apparently that is an escape char */}
                <Link
                    href={"/join"} className="link link-primary">Register
                </Link>
            </p>
        </div>
    );
}
