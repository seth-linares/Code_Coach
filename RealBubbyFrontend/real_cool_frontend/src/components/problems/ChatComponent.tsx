// components/ChatComponent.tsx
"use client";

import React, { useState } from 'react';
import ReactMarkdown from 'react-markdown';
import { useChatGPT } from '@/hooks/useChatGPT';

interface ChatComponentProps {
    problemId: number;
}

const ChatComponent: React.FC<ChatComponentProps> = ({ problemId }) => {
    const { sendMessage, messages, isLoading, error, resetConversation } = useChatGPT(problemId);
    const [inputMessage, setInputMessage] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (inputMessage.trim()) {
            await sendMessage(inputMessage);
            setInputMessage('');
        }
    };

    return (
        <div className="flex-grow flex flex-col">
            <div className="chat-container flex-grow overflow-y-auto mb-2 p-4 bg-base-200 rounded-t-lg">
                {messages.map((msg, index) => (
                    <div key={index} className={`chat ${msg.role === 'user' ? 'chat-end' : 'chat-start'} mb-4`}>
                        <div className={`chat-bubble ${msg.role === 'assistant' ? 'prose' : ''}`}>
                            {msg.role === 'assistant' ? (
                                <ReactMarkdown>{msg.content}</ReactMarkdown>
                            ) : (
                                <pre className="whitespace-pre-wrap">{msg.content}</pre>
                            )}
                        </div>
                    </div>
                ))}
                {isLoading && <div className="chat-start"><div className="chat-bubble">Thinking...</div></div>}
            </div>
            <form onSubmit={handleSubmit} className="input-container">
                <input
                    type="text"
                    value={inputMessage}
                    onChange={(e) => setInputMessage(e.target.value)}
                    placeholder="Chat with AI..."
                    className="input input-bordered w-full"
                    id="user-input"
                />
                <button type="submit" disabled={isLoading} className="btn btn-primary ml-2">Send</button>
            </form>
            {error && (
                <div className="alert alert-error mt-2">
                    <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                    <span>{error}</span>
                    <button onClick={resetConversation} className="btn btn-sm">Reset Conversation</button>
                </div>
            )}
        </div>
    );
};

export default ChatComponent;