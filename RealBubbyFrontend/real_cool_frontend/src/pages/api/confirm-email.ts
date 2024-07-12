import type { NextApiRequest, NextApiResponse } from 'next';
import axios from 'axios';

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
    if (req.method !== 'POST') {
        return res.status(405).json({ message: 'Method not allowed' });
    }

    const { userId, token } = req.body;

    if (!userId || !token) {
        return res.status(400).json({ message: 'Missing userId or token' });
    }

    try {
        const response = await axios.post("https://localhost/api/Users/ConfirmEmail", { userId, token });
        res.status(200).json(response.data);
    } catch (error) {
        console.error('Error confirming email:', error);
        res.status(500).json({ message: 'Failed to confirm email' });
    }
}