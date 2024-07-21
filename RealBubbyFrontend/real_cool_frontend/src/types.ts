// src/types.ts

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