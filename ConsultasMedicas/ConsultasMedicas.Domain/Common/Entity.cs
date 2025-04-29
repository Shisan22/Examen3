namespace ConsultasMedicas.Domain.Common;

// Clase base para Entidades para encapsular el ID
public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    // Constructor sin parámetros para EF Core o serialización, aunque no usemos EF Core ahora.
    protected Entity()
    {
    }
}