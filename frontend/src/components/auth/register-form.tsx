"use client";

import type { ApiErrorResponse, RegisterApiResult } from "@/types/auth";
import { registerSchema, RegisterFormValues } from "@/lib/validations/auth";
import { PasswordField } from "@/components/auth/password-field";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { FormField } from "@/components/auth/form-field";
import { useForm } from "react-hook-form";
import { Loader2 } from "lucide-react";

async function registerRequest(
    values: RegisterFormValues
): Promise<RegisterApiResult> {
    const res = await fetch("http://localhost:8080/api/v1/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(values)
    });

    const data = await res.json();

    console.log(data);

    if (!res.ok) {
        throw new Error(
            (data as ApiErrorResponse).message ?? "ثبت‌نام انجام نشد"
        );
    }

    return data as RegisterApiResult;
}

export function RegisterForm() {
    const router = useRouter();

    const {
        register,
        handleSubmit,
        formState: { errors }
    } = useForm<RegisterFormValues>({
        resolver: zodResolver(registerSchema)
    });

    const mutation = useMutation({
        mutationFn: registerRequest,
        onSuccess: (data) => {
            if (data.autoLogin) {
                router.push("/dashboard");
                router.refresh();
            } else {
                // ثبت‌نام موفق بود ولی لاگین خودکار نشد؛ کاربر رو می‌فرستیم صفحه ورود
                router.push("/login?registered=1");
            }
        }
    });

    const onSubmit = (values: RegisterFormValues) => {
        mutation.mutate(values);
    };

    return (
        <form
            onSubmit={handleSubmit(onSubmit)}
            className="flex flex-col gap-4"
            noValidate
        >
            <div className="grid grid-cols-2 gap-4">
                <FormField
                    id="firstName"
                    label="نام"
                    placeholder="علی"
                    autoComplete="given-name"
                    error={errors.firstName?.message}
                    {...register("firstName")}
                />
                <FormField
                    id="lastName"
                    label="نام‌خانوادگی"
                    placeholder="رضایی"
                    autoComplete="family-name"
                    error={errors.lastName?.message}
                    {...register("lastName")}
                />
            </div>

            <FormField
                id="username"
                label="نام کاربری"
                placeholder="ali_rezaei"
                autoComplete="username"
                dir="ltr"
                className="text-left"
                error={errors.username?.message}
                {...register("username")}
            />

            <FormField
                id="email"
                type="email"
                label="ایمیل"
                placeholder="example@mail.com"
                autoComplete="email"
                dir="ltr"
                className="text-left"
                error={errors.email?.message}
                {...register("email")}
            />

            <PasswordField
                id="password"
                label="رمز عبور"
                placeholder="حداقل ۸ کاراکتر"
                autoComplete="new-password"
                error={errors.password?.message}
                {...register("password")}
            />

            <PasswordField
                id="confirmPassword"
                label="تکرار رمز عبور"
                placeholder="رمز عبور را دوباره وارد کنید"
                autoComplete="new-password"
                error={errors.confirmPassword?.message}
                {...register("confirmPassword")}
            />

            {mutation.isError && (
                <div className="rounded-lg bg-red-50 px-3.5 py-2.5 text-sm text-red-600">
                    {mutation.error instanceof Error
                        ? mutation.error.message
                        : "خطایی رخ داد"}
                </div>
            )}

            <button
                type="submit"
                disabled={mutation.isPending}
                className="mt-2 flex h-11 items-center justify-center gap-2 rounded-lg bg-teal-600 text-sm font-medium text-white transition-colors hover:bg-teal-700 disabled:cursor-not-allowed disabled:opacity-60"
            >
                {mutation.isPending && (
                    <Loader2 size={16} className="animate-spin" />
                )}
                {mutation.isPending ? "در حال ثبت‌نام..." : "ساخت حساب کاربری"}
            </button>
        </form>
    );
}
