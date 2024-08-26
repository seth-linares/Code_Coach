// components/ChatComponent.tsx
"use client";

import React, { useState, useRef, useEffect } from 'react';
import ReactMarkdown from 'react-markdown';
import { useChatGPT } from '@/hooks/useChatGPT';
import {ChatComponentProps} from "@/types";
import remarkGfm from 'remark-gfm';
import rehypeRaw from 'rehype-raw';
import rehypeHighlight from 'rehype-highlight';



const ChatComponent: React.FC<ChatComponentProps> = ( { problemId }: ChatComponentProps ) => {
    const {
        sendMessage,
        messages,
        isLoading,
        error,
        resetConversation
    } = useChatGPT(problemId);

    const [inputMessage, setInputMessage] = useState('');
    const chatContainerRef = useRef<HTMLDivElement>(null);


    // check this one out
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (inputMessage.trim()) {
            await sendMessage(inputMessage);
            setInputMessage('');
        }
    };

    useEffect(() => {
        if (chatContainerRef.current) {
            chatContainerRef.current.scrollTop = chatContainerRef.current.scrollHeight;
        }
    }, [messages]);

    return (
        <div className="flex flex-col h-full">
            <div
                ref={chatContainerRef}
                className="flex-grow overflow-y-auto mb-2 p-4 bg-base-200 rounded-lg"
            >
                {messages.map((msg, index) => (
                    <div key={index} className={`chat ${msg.role === 'user' ? 'chat-end' : 'chat-start'} mb-4`}>
                        <div className={`chat-bubble w-[85%] max-w-[85%] ${msg.role === 'assistant' ? 'prose' : ''}`}>
                            {msg.role === 'assistant' ? (
                                <ReactMarkdown className="whitespace-pre-wrap text-lg" rehypePlugins={[rehypeRaw, rehypeHighlight]} remarkPlugins={[remarkGfm]}>
                                    {msg.content}
                                </ReactMarkdown>
                            ) : (
                                <pre className="whitespace-pre-wrap break-words">{msg.content}</pre>
                            )}
                        </div>
                    </div>
                ))}
                {isLoading && (
                    <div className="chat chat-start mb-4">
                        <div className="chat-bubble w-[85%] max-w-[85%]">Thinking...</div>
                    </div>
                )}
            </div>
            <form onSubmit={handleSubmit} className="flex mt-2">
                <input
                    type="text"
                    value={inputMessage}
                    onChange={(e) => setInputMessage(e.target.value)}
                    placeholder="Chat with AI..."
                    className="input input-bordered flex-grow"
                    id="user-input"
                />
                <button type="submit" disabled={isLoading} className="btn btn-primary ml-2">Send</button>
            </form>
            {error && (
                <div className="alert alert-error mt-2">
                    <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <span>{error}</span>
                    <button onClick={resetConversation} className="btn btn-sm">Reset Conversation</button>
                </div>
            )}
        </div>
    );
};

export default ChatComponent;