// pages/api/login.ts

import axios, { AxiosResponse } from 'axios'; // Import Axios and AxiosResponse for making HTTP requests and typing the responses
import type { NextApiRequest, NextApiResponse } from 'next'; // Import types from Next.js for type checking

// Interface for the login request body
export interface LoginRequestBody {
    username: string;
    password: string;
    rememberMe: boolean;
}

// Interface for the login response
export interface LoginResponse {
    token: string; // The token received upon successful login
}

// Interface for unauthorized errors
export interface UnauthorizedError {
    [key: string]: string[]; // Error messages mapped by field names
}

// The main handler function for the login API route
export default async function handler(
    req: NextApiRequest,
    res: NextApiResponse
): Promise<void> {
    // Check if the request method is POST
    if (req.method === 'POST') {
        try {
            // Extract username and password from the request body
            const { username, password, rememberMe }: LoginRequestBody = req.body;

            // Construct the data object to send to the backend API
            const data: LoginRequestBody = {
                username,
                password,
                rememberMe
            };

            const apiUrl = process.env.NODE_ENV === 'production'
                ? 'https://your-production-domain.com/api/Users/Login' : 'https://localhost/api/Users/Login'

            try {
                // Make a POST request to the backend API to log in the user
                // Ensure the URL matches your backend's login endpoint and uses HTTPS
                const response: AxiosResponse<LoginResponse> = await axios.post<LoginResponse>(apiUrl, data, {
                    headers: {
                        "Content-Type": "application/json"
                    },
                });
                console.log(response.data);
                // Respond with the data received from the backend API (JWT token)
                res.status(200).json(response.data);
            } catch (error: any) {
                // Handle unauthorized errors from the backend API
                if (error.response?.data?.errors) {
                    const unauthorizedError: UnauthorizedError = error.response?.data?.errors;
                    console.log("unauthorizedError: ", unauthorizedError);
                    res.status(400).json(unauthorizedError);
                } else {
                    // Handle other errors
                    res.status(error.response?.status || 500).json({ message: error.message });
                }
            }
        } catch (error: any) {
            // Handle errors in extracting data or making the request
            res.status(500).json({ message: error.message });
        }
    } else {
        // Respond with a 405 Method Not Allowed if the request method is not POST
        res.setHeader('Allow', ['POST']);
        res.status(405).end('Method Not Allowed');
    }
}
