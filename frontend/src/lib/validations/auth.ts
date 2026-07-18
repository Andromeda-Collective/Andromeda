import { z } from "zod";

export const registerSchema = z
    .object({
        firstName: z.string().min(2, "نام باید حداقل ۲ کاراکتر باشد").max(50),
        lastName: z
            .string()
            .min(2, "نام‌خانوادگی باید حداقل ۲ کاراکتر باشد")
            .max(50),
        username: z
            .string()
            .min(3, "نام کاربری باید حداقل ۳ کاراکتر باشد")
            .max(30, "نام کاربری خیلی طولانی است")
            .regex(
                /^[a-zA-Z0-9_]+$/,
                "نام کاربری فقط می‌تواند شامل حروف انگلیسی، عدد و _ باشد"
            ),
        email: z
            .string()
            .min(1, "ایمیل را وارد کنید")
            .email("ایمیل معتبر نیست"),
        password: z
            .string()
            .min(8, "رمز عبور باید حداقل ۸ کاراکتر باشد")
            .regex(/[A-Z]/, "رمز عبور باید حداقل یک حرف بزرگ داشته باشد")
            .regex(/[0-9]/, "رمز عبور باید حداقل یک عدد داشته باشد"),
        confirmPassword: z.string().min(1, "تکرار رمز عبور را وارد کنید")
    })
    .refine((data) => data.password === data.confirmPassword, {
        message: "رمز عبور و تکرار آن یکسان نیستند",
        path: ["confirmPassword"]
    });

export type RegisterFormValues = z.infer<typeof registerSchema>;
