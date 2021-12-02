namespace Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public virtual User User { get; set; }
    public string Token { get; set; }
    public DateTimeOffset Expires { get; set; } = DateTimeOffset.UtcNow.AddDays(7);
    public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
    public DateTimeOffset? Revoked { get; set; }
    public bool IsActive => Revoked == null & !IsExpired;
}

