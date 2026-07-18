import { NextResponse } from "next/server";
import type { AuthTokenResponse } from "@/types/auth";

const isProd = process.env.NODE_ENV === "production";

/**
 * توکن‌های دریافتی از دات‌نت رو به‌عنوان httpOnly cookie روی response ست می‌کنه.
 * userId هم جداگانه نگه داشته می‌شه چون endpoint رفرش دات‌نت هم userId هم
 * refreshToken می‌خواد (نه فقط refreshToken).
 */
export function setAuthCookies(
    response: NextResponse,
    data: AuthTokenResponse
) {
    const accessMaxAge = Math.max(
        Math.floor((new Date(data.expiresAt).getTime() - Date.now()) / 1000),
        60
    );

    response.cookies.set("access_token", data.accessToken, {
        httpOnly: true,
        secure: isProd,
        sameSite: "lax",
        path: "/",
        maxAge: accessMaxAge
    });

    response.cookies.set("refresh_token", data.refreshToken, {
        httpOnly: true,
        secure: isProd,
        sameSite: "lax",
        path: "/",
        maxAge: 60 * 60 * 24 * 30 // ۳۰ روز
    });

    response.cookies.set("user_id", data.userId, {
        httpOnly: true,
        secure: isProd,
        sameSite: "lax",
        path: "/",
        maxAge: 60 * 60 * 24 * 30
    });

    return response;
}

export function clearAuthCookies(response: NextResponse) {
    response.cookies.delete("access_token");
    response.cookies.delete("refresh_token");
    response.cookies.delete("user_id");
    return response;
}
