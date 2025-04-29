namespace ConsultasMedicas.Domain.Common;

// Clase base para las Raíces de Agregado (puede añadir manejo de eventos de dominio si es necesario)
public abstract class AggregateRoot : Entity
{
    // Lista para almacenar eventos de dominio. Se puede implementar más adelante si es necesario.
    // private readonly List<IDomainEvent> _domainEvents = new();
    // public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    // protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    // public void ClearDomainEvents() => _domainEvents.Clear();

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}