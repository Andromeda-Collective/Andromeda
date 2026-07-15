namespace Andromeda.Entities;

public sealed class Token
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenValue { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = null!;
}