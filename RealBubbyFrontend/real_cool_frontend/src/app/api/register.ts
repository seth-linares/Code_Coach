// join/api/register.ts

import axios from "axios";
import type { NextApiRequest, NextApiResponse } from "next";

export interface RegisterRequestBody {
    username: string,
    password: string,
    comparePassword: string,
    emailAddress: string,
}


export default async function handler(
    req: NextApiRequest,
    res: NextApiResponse
) {
    if(req.method === "POST") {
        try {
            // deconstruct into the type expected by DTO in ASP
            const {username, password, comparePassword, emailAddress}: RegisterRequestBody = req.body;

            // construct the DTO for the POST request to the ASP backend
            const data = {
                Username: username,
                Password: password,
                ComparePassword: comparePassword,
                EmailAddress: emailAddress,
            };

            // make http post request to the backend
            const response = await axios.post("https://localhost:32768/api/Users/Register", data);

            // respond with data from the backend (JWT token)
            res.status(201).json(response.data);
        }

        catch(error: any) {
            // handle the error
            res.status(error.response?.status || 500).json({ message: error.message });
        }
    }
    else {
        // handle non-post
        res.setHeader("Allow", ['POST']);
        res.status(405).end("Method Not Allowed");
    }
}