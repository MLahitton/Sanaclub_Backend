namespace Sanaclub.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    protected BaseEntity()
    {
    }

    protected BaseEntity(Guid id)
    {
        Id = id == Guid.Empty
            ? throw new DomainException("El identificador no puede estar vacío.")
            : id;
    }
}