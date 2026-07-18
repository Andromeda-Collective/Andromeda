using Andromeda.Common.Services.FileStorage;
using Andromeda.Enums;
using Microsoft.AspNetCore.Identity;

namespace Andromeda.Entities;


public sealed class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserState State { get; set; }
    public string ProfileImagePath { get; set; } = ProfileImageDefaults.DefaultImagePath;
}