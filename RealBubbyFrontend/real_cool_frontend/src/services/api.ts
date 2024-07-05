import axios, {AxiosError, AxiosInstance} from 'axios';

export interface ApiResponse<T> {
    data?: T;
    error?: string;
    validationErrors?: Record<string, string[]>;
}

interface ErrorResponse {
    message?: string;
    errors?: Record<string, string[]>;
}

const api: AxiosInstance = axios.create({
    baseURL: 'https://localhost/api',
    withCredentials: true,
});

api.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
        const apiError: ApiResponse<any> = {};

        if (error.response) {
            const errorData: ErrorResponse = error.response.data as ErrorResponse;

            if (errorData.errors) {
                apiError.validationErrors = errorData.errors;
            } else if (errorData.message) {
                apiError.error = errorData.message;
            } else {
                apiError.error = 'An unknown error occurred';
            }
        } else if (error.request) {
            apiError.error = 'No response received from the server';
        } else {
            apiError.error = error.message || 'An unknown error occurred';
        }

        return Promise.reject(apiError);
    }
);

export default api;