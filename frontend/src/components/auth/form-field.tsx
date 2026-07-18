import { InputHTMLAttributes, forwardRef } from "react";

interface FormFieldProps extends InputHTMLAttributes<HTMLInputElement> {
    label: string;
    error?: string;
}

export const FormField = forwardRef<HTMLInputElement, FormFieldProps>(
    ({ label, error, id, className = "", ...props }, ref) => {
        return (
            <div className="flex flex-col gap-1.5">
                <label
                    htmlFor={id}
                    className="text-sm font-medium text-slate-700"
                >
                    {label}
                </label>
                <input
                    ref={ref}
                    id={id}
                    className={`h-11 rounded-lg border bg-white px-3.5 text-sm text-slate-900 outline-none transition-colors placeholder:text-slate-400 focus:ring-2 focus:ring-offset-0 ${
                        error
                            ? "border-red-400 focus:border-red-400 focus:ring-red-100"
                            : "border-slate-200 focus:border-teal-500 focus:ring-teal-100"
                    } ${className}`}
                    aria-invalid={!!error}
                    aria-describedby={error ? `${id}-error` : undefined}
                    {...props}
                />
                {error && (
                    <p id={`${id}-error`} className="text-xs text-red-500">
                        {error}
                    </p>
                )}
            </div>
        );
    }
);

FormField.displayName = "FormField";
