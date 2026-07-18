using Andromeda.Common;

namespace Andromeda.Features.Auth;

public static class AuthErrors
{
    private const string Prefix = "Auth";

    public static readonly Error InvalidCredentials =
        Error.Unauthorized($"{Prefix}.{InvalidCredentials}", "ایمیل یا رمز عبور اشتباه است");

    public static readonly Error UserNotActive =
        Error.Forbidden($"{Prefix}.{UserNotActive}", "حساب کاربری فعال نیست");

    public static readonly Error UserLockedOut =
        Error.Forbidden($"{Prefix}.{UserLockedOut}", "حساب کاربری قفل شده است");

    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized($"{Prefix}.{InvalidRefreshToken}", "رفرش توکن نامعتبر است");

    public static readonly Error RefreshTokenExpired =
        Error.Unauthorized($"{Prefix}.{RefreshTokenExpired}", "رفرش توکن منقضی شده است");

    public static readonly Error RefreshTokenRevoked =
        Error.Unauthorized($"{Prefix}.{RefreshTokenRevoked}", "رفرش توکن باطل شده است");

    public static readonly Error UserNotFound =
        Error.NotFound($"{Prefix}.{UserNotFound}", "کاربر یافت نشد");

    public static readonly Error AlreadyAuthenticated =
        Error.Conflict($"{Prefix}.{AlreadyAuthenticated}", "شما از قبل وارد سیستم شده‌اید");
}