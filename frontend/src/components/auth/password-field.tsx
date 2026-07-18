"use client";

import { InputHTMLAttributes, forwardRef, useState } from "react";
import { Eye, EyeOff } from "lucide-react";

interface PasswordFieldProps extends InputHTMLAttributes<HTMLInputElement> {
    label: string;
    error?: string;
}

export const PasswordField = forwardRef<HTMLInputElement, PasswordFieldProps>(
    ({ label, error, id, className = "", ...props }, ref) => {
        const [visible, setVisible] = useState(false);

        return (
            <div className="flex flex-col gap-1.5">
                <label
                    htmlFor={id}
                    className="text-sm font-medium text-slate-700"
                >
                    {label}
                </label>
                <div className="relative">
                    <input
                        ref={ref}
                        id={id}
                        type={visible ? "text" : "password"}
                        className={`h-11 w-full rounded-lg border bg-white px-3.5 pe-11 text-sm text-slate-900 outline-none transition-colors placeholder:text-slate-400 focus:ring-2 focus:ring-offset-0 ${
                            error
                                ? "border-red-400 focus:border-red-400 focus:ring-red-100"
                                : "border-slate-200 focus:border-teal-500 focus:ring-teal-100"
                        } ${className}`}
                        aria-invalid={!!error}
                        aria-describedby={error ? `${id}-error` : undefined}
                        {...props}
                    />
                    <button
                        type="button"
                        onClick={() => setVisible((v) => !v)}
                        className="absolute inset-y-0 end-0 flex w-11 items-center justify-center text-slate-400 hover:text-slate-600"
                        tabIndex={-1}
                        aria-label={
                            visible ? "پنهان کردن رمز عبور" : "نمایش رمز عبور"
                        }
                    >
                        {visible ? <EyeOff size={18} /> : <Eye size={18} />}
                    </button>
                </div>
                {error && (
                    <p id={`${id}-error`} className="text-xs text-red-500">
                        {error}
                    </p>
                )}
            </div>
        );
    }
);

PasswordField.displayName = "PasswordField";
