import { NextResponse } from "next/server";

import { setAuthCookies } from "@/lib/auth/cookies";
import type { AuthTokenResponse, RegisterPayload } from "@/types/auth";

const DOTNET_API_URL = process.env.DOTNET_API_URL;

/**
 * خطاهای دات‌نت به فرمت application/problem+json (RFC 7807) میان.
 * برای خطاهای validation معمولاً یه شیء `errors` هم داخلشه.
 */
async function parseApiError(res: Response): Promise<string> {
    try {
        const problem = await res.json();
        if (problem?.errors) {
            const firstField = Object.values(problem.errors)[0];
            if (Array.isArray(firstField) && firstField[0]) {
                return firstField[0] as string;
            }
        }
        return problem?.detail || problem?.title || "خطایی رخ داد";
    } catch {
        return "خطایی رخ داد";
    }
}

export async function POST(req: Request) {
    // ۰. مطمئن شو آدرس API ست شده — قبل از هر فچی
    if (!DOTNET_API_URL) {
        console.error(
            "DOTNET_API_URL is not set. Check your .env.local file and restart the dev server."
        );
        return NextResponse.json(
            { message: "پیکربندی سرور ناقص است (DOTNET_API_URL تنظیم نشده)" },
            { status: 500 }
        );
    }

    let body: RegisterPayload;
    try {
        body = await req.json();
    } catch {
        return NextResponse.json(
            { message: "بدنه درخواست نامعتبر است" },
            { status: 400 }
        );
    }

    try {
        // ۱. ثبت‌نام — بدون توکن، بدون بدنه در پاسخ موفق
        const registerRes = await fetch(
            `${DOTNET_API_URL}/api/v1/auth/register`,
            {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body),
                cache: "no-store"
            }
        );

        if (!registerRes.ok) {
            const message = await parseApiError(registerRes);
            return NextResponse.json(
                { message },
                { status: registerRes.status }
            );
        }

        // ۲. auto-login بعد از ثبت‌نام موفق
        const loginRes = await fetch(`${DOTNET_API_URL}/api/v1/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                email: body.email,
                password: body.password
            }),
            cache: "no-store"
        });

        if (!loginRes.ok) {
            return NextResponse.json(
                { success: true, autoLogin: false },
                { status: 201 }
            );
        }

        const loginData: AuthTokenResponse = await loginRes.json();

        const response = NextResponse.json(
            {
                success: true,
                autoLogin: true,
                user: {
                    id: loginData.userId,
                    username: loginData.username,
                    email: loginData.email
                }
            },
            { status: 201 }
        );

        return setAuthCookies(response, loginData);
    } catch (error) {
        // اینجا یعنی خود fetch fail کرده (سرور دات‌نت خاموشه، آدرس اشتباهه، شبکه قطعه و ...)
        console.error("Register route failed to reach .NET API:", error);
        return NextResponse.json(
            {
                message: "امکان ارتباط با سرور وجود ندارد."
            },
            { status: 502 }
        );
    }
}
