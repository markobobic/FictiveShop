using NUlid;

namespace FictiveShop.Core.Domain
{
    public abstract class BaseEntity
    {
        public string Id { get; init; } = Ulid.NewUlid().ToString();
    }
}