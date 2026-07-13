using Andromeda.Enums;

namespace Andromeda.Common;

public interface IValidationResult
{
    public static readonly Error ValidationError = new(
        "Validation Error",
        "A validation problem occurred", ErrorType.Validation
    );

    Error[] Errors { get; }
}