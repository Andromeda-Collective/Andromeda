using Andromeda.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Andromeda.Data;

public sealed class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Token> Tokens => Set<Token>();





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region User

        modelBuilder.Entity<User>()
            .ToTable("Users");

        modelBuilder.Entity<User>()
            .Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<User>()
            .Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<User>()
            .Property(user => user.State)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(user => user.ProfileImagePath)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasIndex(x => x.NormalizedEmail)
            .IsUnique();

        #endregion

        #region Role

        modelBuilder.Entity<Role>()
            .ToTable("Roles");

        #endregion

        #region Identity

        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("UserRoles");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("UserClaims");

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("RoleClaims");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("UserLogins");

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("UserTokens");

        #endregion

        #region Token

        modelBuilder.Entity<Token>()
            .ToTable("Tokens");

        modelBuilder.Entity<Token>()
            .Property(token => token.TokenValue)
            .IsRequired()
            .HasMaxLength(500);

        modelBuilder.Entity<Token>()
            .Property(token => token.UserId)
            .IsRequired();

        modelBuilder.Entity<Token>()
            .HasIndex(token => token.UserId);

        modelBuilder.Entity<Token>()
            .HasIndex(token => token.TokenValue)
            .IsUnique();

        modelBuilder.Entity<Token>()
            .HasIndex(token => token.ExpiresAt);

        modelBuilder.Entity<Token>()
            .HasIndex(token => new { token.UserId, token.IsRevoked });

        modelBuilder.Entity<Token>()
            .HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion





    }
}