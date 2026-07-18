import type { Metadata } from "next";
import Link from "next/link";
import { RegisterForm } from "@/components/auth/register-form";

export const metadata: Metadata = {
    title: "ثبت‌نام | تیم ما",
    description: "ساخت حساب کاربری جدید"
};

export default function RegisterPage() {
    return (
        <div className="w-full max-w-md">
            <div className="rounded-2xl border border-slate-200 bg-white p-8 shadow-sm">
                <div className="mb-8 text-center">
                    <h1 className="text-xl font-bold text-slate-900">
                        ساخت حساب کاربری
                    </h1>
                    <p className="mt-1.5 text-sm text-slate-500">
                        برای شروع، اطلاعات زیر را تکمیل کنید
                    </p>
                </div>

                <RegisterForm />

                <p className="mt-6 text-center text-sm text-slate-500">
                    قبلاً حساب ساخته‌اید؟{" "}
                    <Link
                        href="/login"
                        className="font-medium text-teal-600 hover:underline"
                    >
                        وارد شوید
                    </Link>
                </p>
            </div>
        </div>
    );
}
