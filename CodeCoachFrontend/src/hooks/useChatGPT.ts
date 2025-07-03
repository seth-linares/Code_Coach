// src/hooks/useChatGPT.ts

import { useState, useCallback } from 'react';
import axios from 'axios';
import { ChatGPTRequest, ChatGPTResponse, UseChatGPTResult } from "@/types";

const API_URL = 'https://www.codecoachapp.com/api/AIConversations/ChatGPT';

export const useChatGPT = (problemId: number): UseChatGPTResult => {
    const [conversationId, setConversationId] = useState<number | undefined>(undefined);
    const [messages, setMessages] = useState<{ role: 'user' | 'assistant', content: string }[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const sendMessage = useCallback(async (message: string) => {
        setIsLoading(true);
        setError(null);

        try {
            const request: ChatGPTRequest = {
                conversationId,
                problemId,
                message: btoa(message) // Base64 encode the message
            };

            const result = await axios.post<ChatGPTResponse>(API_URL, request, {
                withCredentials: true,
            });

            if (result.data.conversationId) {
                setConversationId(result.data.conversationId);
            }

            if (result.data.message) {
                setMessages(prevMessages => [
                    ...prevMessages,
                    { role: 'user', content: message },
                    { role: 'assistant', content: result.data.message }
                ]);
            } else {
                throw new Error('Received empty response from server');
            }
        } catch (err) {
            if (axios.isAxiosError(err)) {
                if (err.response?.status === 401) {
                    setError('Unauthorized. Please check your API key.');
                } else if (err.response?.status === 404) {
                    setError('Conversation or problem not found.');
                } else {
                    setError(err.response?.data || 'An error occurred while processing your request');
                }
            } else {
                setError('An unexpected error occurred. Please try again.');
            }
        } finally {
            setIsLoading(false);
        }
    }, [conversationId, problemId]);

    const resetConversation = useCallback(() => {
        setConversationId(undefined);
        setMessages([]);
        setError(null);
    }, []);

    return { sendMessage, messages, isLoading, error, resetConversation };
};