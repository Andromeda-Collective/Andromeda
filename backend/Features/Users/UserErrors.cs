using Andromeda.Common;

namespace Andromeda.Features.Users;

public static class UserErrors
{
    private const string Prefix = "User";

    public static readonly Error UserNotFound =
        Error.NotFound($"{Prefix}.{UserNotFound}", "کاربر یافت نشد");

    public static readonly Error CannotModifyOwner =
        Error.Forbidden($"{Prefix}.{CannotModifyOwner}", "امکان تغییر وضعیت یا رول Owner وجود ندارد");

    public static readonly Error CannotAssignRole =
        Error.Forbidden($"{Prefix}.{CannotAssignRole}", "شما اجازه‌ی ایجاد کاربر با این رول را ندارید");

    public static readonly Error CannotLogoutTarget =
        Error.Forbidden($"{Prefix}.{CannotLogoutTarget}", "شما اجازه‌ی خروج اجباری این کاربر را ندارید");

    public static readonly Error PasswordChangeFailed =
        Error.Failure($"{Prefix}.{PasswordChangeFailed}", "تغییر رمز عبور با خطا مواجه شد");

    public static readonly Error InvalidImage =
        Error.Validation($"{Prefix}.{InvalidImage}", "فایل تصویر نامعتبر است");

    public static readonly Error UpdateFailed =
        Error.Failure($"{Prefix}.{UpdateFailed}", "به‌روزرسانی پروفایل با خطا مواجه شد");
}