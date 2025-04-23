using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EpicShowdown.API.Data.Base;

public interface IEntity<TKey>
{
    TKey Id { get; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

public abstract class Entity<TKey> : IEntity<TKey>
{
    [Required]
    public virtual TKey Id { get; protected set; } = default!;

    private int? _requestedHashCode;

    public bool IsTransient()
    {
        return Id?.Equals(default(TKey)) ?? true;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || !(obj is Entity<TKey>))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        var item = (Entity<TKey>)obj;

        if (item.IsTransient() || IsTransient())
            return false;
        else
            return item == this;
    }

    public override int GetHashCode()
    {
        if (!_requestedHashCode.HasValue)
        {
            _requestedHashCode = Id?.GetHashCode() ?? 0 ^ 31;

            return _requestedHashCode.Value;
        }
        else
            return base.GetHashCode();
    }

    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
    {
        if (Equals(left, null))
            return Equals(right, null) ? true : false;
        else
            return left.Equals(right);
    }

    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
    {
        return !(left == right);
    }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}