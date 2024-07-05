import api, { ApiResponse } from './api';
import {AxiosResponse} from "axios";

// Types
export interface RegisterRequest {
    username: string;
    password: string;
    confirmPassword: string;
    emailAddress: string;
}

export interface LoginRequestBody {
    username: string;
    password: string;
}

export interface AuthResponse { // need to use token in a cookie, or maybe soon the response will be the cookie? Something like that
    message: string;
}

// Functions
export const register = async (userData: RegisterRequest): Promise<ApiResponse<AuthResponse>> => {
    try {
        const response = await api.post<AuthResponse>('/Users/Register', userData, { withCredentials: true });
        return { data: response.data };
    } catch (error) {
        return error as ApiResponse<AuthResponse>;
    }
};

// Add other auth-related functions here (login, logout, etc.)