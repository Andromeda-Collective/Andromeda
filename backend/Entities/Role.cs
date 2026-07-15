using Microsoft.AspNetCore.Identity;

namespace Andromeda.Entities;


public sealed class Role : IdentityRole<Guid>
{
    public Role(string role) : base(role) {}
    public Role() {}
}