using NUlid;

namespace FictiveShop.Core.Domain
{
    public abstract class BaseEntity
    {
        public Ulid Id { get; init; } = Ulid.NewUlid();
    }
}