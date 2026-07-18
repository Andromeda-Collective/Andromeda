export interface RegisterPayload {
    firstName: string;
    lastName: string;
    email: string;
    username: string;
    password: string;
    confirmPassword: string;
}

export interface LoginPayload {
    email: string;
    password: string;
}

// پاسخ endpoint لاگین دات‌نت
export interface AuthTokenResponse {
    userId: string;
    accessToken: string;
    refreshToken: string;
    expiresAt: string; // ISO date string
    username: string;
    email: string;
}

// پاسخی که خود Route Handler ما به کلاینت می‌ده
export interface RegisterApiResult {
    success: true;
    autoLogin: boolean;
    user?: { id: string; username: string; email: string };
}

export interface ApiErrorResponse {
    message: string;
}
