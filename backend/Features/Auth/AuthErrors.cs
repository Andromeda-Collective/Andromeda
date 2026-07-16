using Andromeda.Common;

namespace Andromeda.Features.Auth;

public static class AuthErrors
{
    public static readonly Error EmailAlreadyExists =
        Error.Conflict("Auth.EmailAlreadyExists", "این ایمیل قبلا ثبت شده است");

    public static readonly Error UsernameAlreadyExists =
        Error.Conflict("Auth.UsernameAlreadyExists", "این نام کاربری قبلا گرفته شده است");

    public static readonly Error RegistrationFailed =
        Error.Failure("Auth.RegistrationFailed", "ثبت‌نام با خطا مواجه شد");

    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", "ایمیل یا رمز عبور اشتباه است");

    public static readonly Error UserNotActive =
        Error.Forbidden("Auth.UserNotActive", "حساب کاربری فعال نیست");

    public static readonly Error UserLockedOut =
        Error.Forbidden("Auth.UserLockedOut", "حساب کاربری قفل شده است");

    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized("Auth.InvalidRefreshToken", "رفرش توکن نامعتبر است");

    public static readonly Error RefreshTokenExpired =
        Error.Unauthorized("Auth.RefreshTokenExpired", "رفرش توکن منقضی شده است");

    public static readonly Error RefreshTokenRevoked =
        Error.Unauthorized("Auth.RefreshTokenRevoked", "رفرش توکن باطل شده است");

    public static readonly Error UserNotFound =
        Error.NotFound("Auth.UserNotFound", "کاربر یافت نشد");

    public static readonly Error AlreadyAuthenticated =
        Error.Conflict("Auth.AlreadyAuthenticated", "شما از قبل وارد سیستم شده‌اید");
}