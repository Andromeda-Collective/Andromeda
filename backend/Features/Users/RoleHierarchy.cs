using Andromeda.Enums;

namespace Andromeda.Features.Users;

public static class RoleHierarchy
{
    public static bool CanCreateWithRole(string callerRole, Roles targetRole) => callerRole switch
    {
        nameof(Roles.Owner) => targetRole is Roles.Admin or Roles.User,
        nameof(Roles.Admin) => targetRole is Roles.User,
        _ => false
    };

    public static bool CanChangeStatus(string callerRole, string targetRole)
    {
        if (targetRole == nameof(Roles.Owner)) return false;

        return callerRole switch
        {
            nameof(Roles.Owner) => true,
            nameof(Roles.Admin) => targetRole == nameof(Roles.User),
            _ => false
        };
    }

    public static bool CanEditFullProfile(string callerRole, string targetRole)
        => callerRole == nameof(Roles.Owner) && targetRole != nameof(Roles.Owner);

    public static bool CanLogoutTarget(string callerRole, string targetRole)
    {
        if (targetRole == nameof(Roles.Owner)) return false;

        return callerRole switch
        {
            nameof(Roles.Owner) => true,
            nameof(Roles.Admin) => targetRole == nameof(Roles.User),
            _ => false
        };
    }
}