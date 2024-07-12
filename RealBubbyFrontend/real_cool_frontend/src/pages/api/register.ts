// pages/api/register.ts

import type { NextApiRequest, NextApiResponse } from 'next';
import axios, { AxiosError } from 'axios';

export interface RegisterRequest {
    username: string;
    password: string;
    confirmPassword: string;
    emailAddress: string;
}

export interface RegisterResponse {
    message: string;
    userId: string;
}

export interface ValidationError {
    [key: string]: string[];
}

const apiClient = axios.create({
    baseURL: process.env.NODE_ENV === 'production'
        ? 'https://your-production-domain.com/api'
        : 'https://localhost/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

export default async function handler(
    req: NextApiRequest,
    res: NextApiResponse
): Promise<void> {
    if (req.method !== 'POST') {
        res.setHeader('Allow', ['POST']);
        res.status(405).end('Method Not Allowed');
        return;
    }

    try {
        const { username, password, confirmPassword, emailAddress }: RegisterRequest = req.body;

        const response = await apiClient.post<RegisterResponse>('/Users/Register', {
            username,
            password,
            confirmPassword,
            emailAddress,
        });

        res.status(201).json(response.data);
    } catch (error) {
        if (axios.isAxiosError(error)) {
            const axiosError = error as AxiosError<{ errors?: ValidationError }>;
            if (axiosError.response?.status === 400) {
                // Handle validation errors
                const validationErrors = axiosError.response.data.errors || {};
                res.status(400).json({ errors: validationErrors });
            } else if (axiosError.response?.status === 500) {
                // Handle server errors
                res.status(500).json({ message: axiosError.response.data });
            } else {
                // Handle other errors
                res.status(axiosError.response?.status || 500).json({ message: axiosError.message });
            }
        } else {
            res.status(500).json({ message: 'An unexpected error occurred' });
        }
    }
}