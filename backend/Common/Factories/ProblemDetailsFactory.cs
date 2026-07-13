using Microsoft.AspNetCore.Mvc;

namespace Andromeda.Common.Factories;

public static class ProblemDetailsFactory
{
    public static ProblemDetails CreateProblemDetails(
    string title,
    int status,
    Error error,
    Error[]? errors = null
    ) => new()
    {
        Title = title,
        Type = error.Code,
        Detail = error.Description,
        Status = status,
        Extensions = { { nameof(errors), errors } },
    };
}