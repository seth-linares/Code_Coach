import api, { ApiResponse } from './api';

export interface RegisterRequest {
    username: string;
    password: string;
    confirmPassword: string;
    emailAddress: string;
}

export interface RegisterResponse {
    token: string;
}

export const register = async (userData: RegisterRequest): Promise<ApiResponse<RegisterResponse>> => {
    try {
        const response = await api.post<RegisterResponse>('/Users/Register', userData);
        return { data: response.data };
    } catch (error) {
        return error as ApiResponse<RegisterResponse>;
    }
};

// Add other auth-related functions here (login, logout, etc.)