// src/types.ts

import React from "react";

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

export interface UserStats {
    username: string;
    totalScore: number;
    rank: string;
    completedProblems: number;
    attemptedProblems: number;
    profilePictureURL: string;
    registrationDate: string;
}

export interface TwoFactorStatus {
    is2faEnabled: boolean;
}

export interface ApiResponse<T> {
    data: T | null;
    error: string | null;
}

export interface TwoFactorSectionProps {
    status: TwoFactorStatus | null;
    getStatus: () => Promise<void>;
    enable2FA: () => Promise<string | undefined>;
    verify2FA: (code: string) => Promise<string | undefined>;
    disable2FA: () => Promise<string | undefined>;
    loading: boolean;
    error: string | null;
}

export interface VerificationModalProps {
    isOpen: boolean;
    onClose: () => void;
    onVerify: () => void;
    verificationCode: string;
    setVerificationCode: (code: string) => void;
    loading: boolean;
    error: string | null;
}

export interface APIKey {
    apiKeyID: number;
    keyName: string;
    isActive: boolean;
    createdAt: string;
    lastUsedAt: string | null;
    usageCount: number;
}

export interface CreateAPIKeyRequest {
    KeyName: string;
    KeyValue: string;
}


export interface UpdateAPIKeyRequest {
    APIKeyID: number;
    KeyName: string;
    KeyValue: string;
}

export interface RequestId {
    Id: number;
}


export interface APIKeyModalProps {
    isOpen: boolean;
    onClose: () => void;
    title: string;
    keyName: string;
    keyValue: string;
    onKeyNameChange: (value: string) => void;
    onKeyValueChange: (value: string) => void;
    onSubmit: () => void;
    submitText: string;
}

export interface APIKeyTableProps {
    apiKeys: APIKey[];
    onSetActive: (id: number) => void;
    onEdit: (key: APIKey) => void;
    onDelete: (id: number) => void;
}

export interface ErrorResponse {
    message?: string;
}

export interface ChangePasswordDto {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

export interface UseProfileActionsResult {
    loading: boolean;
    error: string | null;
    message: string | null;
    validationErrors: Record<string, string[]> | null;
    changePassword: (passwordDto: ChangePasswordDto) => Promise<void>;
    logout: () => Promise<void>;
}

export interface GetProblemsRequest {
    difficulty: string;
    category: string;
}

export interface GetProblemsResponse {
    problemID: number;
    isCompleted: boolean;
    title: string;
    difficulty: string;
    category: string;
    points: string;
}

export interface ProblemLanguageDetails {
    languageID: number;
    languageName: string;
    judge0LanguageId: number;
    functionSignature: string;
}

export interface ProblemDetails {
    problemID: number;
    title: string;
    description: string;
    difficulty: number;
    category: number;
    points: number;
    languageDetails: ProblemLanguageDetails[];
}

export interface SubmissionResult {
    isSuccessful: boolean;
    stdout?: string;
    stderr?: string;
    compileOutput?: string;
    executionTime?: number;
    memoryUsed?: number;
    status: {
        id: number;
        description: string;
    };
}

export interface SubmissionError {
    message: string;
    status: number;
}

export interface ModalProps {
    isOpen: boolean;
    onClose: () => void;
    title: string;
    children: React.ReactNode;
}


export interface ChatGPTRequest {
    conversationId?: number;
    problemId: number;
    message: string;
}

export interface ChatGPTResponse {
    conversationId: number;
    message: string;
}

export interface UseChatGPTResult {
    sendMessage: (message: string) => Promise<void>;
    messages: { role: 'user' | 'assistant', content: string }[];
    isLoading: boolean;
    error: string | null;
    resetConversation: () => void;
}

export interface ChatComponentProps {
    problemId: number;
}

export interface CodeEditorProps {
    problemId: number;
    languageDetails: ProblemLanguageDetails[];
}

export interface EditorState {
    activeLanguage: number | null;
    codeByLanguage: Record<number, string>;
}