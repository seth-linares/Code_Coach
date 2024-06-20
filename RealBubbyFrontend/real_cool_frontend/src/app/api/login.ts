// pages/api/login.ts

import axios from 'axios';
import type { NextApiRequest, NextApiResponse } from 'next';

export interface LoginRequestBody {
    username: string;
    password: string;
}

export default async function handler(
    req: NextApiRequest,
    res: NextApiResponse
) {
    if (req.method === 'POST') {
        try {
            // Deconstruct into the type expected by DTO in ASP.NET
            const { username, password }: LoginRequestBody = req.body;

            // Construct the DTO for the POST request to the ASP.NET backend
            const data = {
                Username: username,
                Password: password,
            };

            // Make HTTP POST request to the backend
            const response = await axios.post("https://localhost:32768/api/Users/Login", data);

            // Respond with data from the backend (JWT token)
            res.status(200).json(response.data);
        } catch (error: any) {
            // Handle the error
            res.status(error.response?.status || 500).json({ message: error.message });
        }
    } else {
        // Handle non-POST
        res.setHeader('Allow', ['POST']);
        res.status(405).end('Method Not Allowed');
    }
}
