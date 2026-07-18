
namespace Andromeda.Common.Errors;

public static class CommonErrors
{
    private const string Prefix = "Common";

    public static readonly Error EmailAlreadyExists =
        Error.Conflict($"{Prefix}.{EmailAlreadyExists}", "این ایمیل قبلا ثبت شده است");

    public static readonly Error UsernameAlreadyExists =
        Error.Conflict($"{Prefix}.{UsernameAlreadyExists}", "این نام کاربری قبلا گرفته شده است");

    public static readonly Error RegistrationFailed =
        Error.Failure($"{Prefix}.{RegistrationFailed}", "ثبت‌نام با خطا مواجه شد");
}