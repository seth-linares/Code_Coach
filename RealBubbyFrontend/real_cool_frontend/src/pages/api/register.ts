// pages/api/register.ts

import axios, { AxiosResponse } from 'axios';
import type { NextApiRequest, NextApiResponse } from 'next';

export interface RegisterRequest {
    username: string;
    password: string;
    confirmPassword: string;
    emailAddress: string;
}

export interface RegisterResponse {
    token: string;
}

export interface ValidationError {
    [key: string]: string[];
}

export default async function handler(
    req: NextApiRequest,
    res: NextApiResponse
): Promise<void> {
    if (req.method === 'POST') {
        try {
            const { username, password, confirmPassword, emailAddress }: RegisterRequest = req.body;

            const data: RegisterRequest = {
                username,
                password,
                confirmPassword,
                emailAddress,
            };

            // Update the URL to use HTTPS and the new Nginx proxy
            const apiUrl = process.env.NODE_ENV === 'production'
                ? 'https://your-production-domain.com/api/Users/Register'
                : 'https://localhost/api/Users/Register';

            try {
                const response: AxiosResponse<RegisterResponse> = await axios.post<RegisterResponse>(apiUrl, data, {
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });
                console.log(response.data);
                res.status(201).json(response.data);
            } catch (error: any) {
                if (error.response?.data?.errors) {
                    const validationError: ValidationError = error.response?.data?.errors;
                    console.log("validationError", validationError);
                    res.status(400).json({ errors: validationError });
                } else {
                    res.status(error.response?.status || 500).json({ message: error.message });
                }
            }
        } catch (error: any) {
            res.status(500).json({ message: error.message });
        }
    } else {
        res.setHeader('Allow', ['POST']);
        res.status(405).end('Method Not Allowed');
    }
}