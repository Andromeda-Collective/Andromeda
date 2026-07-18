
namespace Andromeda.Common.Services.FileStorage;

public static class FileStorageErrors
{
    private const string Prefix = "FileStorage";
    public static readonly Error EmptyFile =
        Error.Validation($"{Prefix}.{EmptyFile}", "فایل نمیتواند خالی باشد.");

    public static readonly Error MaxFileSize =
        Error.Validation($"{Prefix}.{MaxFileSize}", "حجم فایل نباید بیشتر از ۲ مگابایت باشد");

    public static readonly Error FileFormat =
        Error.Validation($"{Prefix}.{FileFormat}", "فرمت فایل مجاز نیست (فقط jpg، jpeg، png، webp)");
}