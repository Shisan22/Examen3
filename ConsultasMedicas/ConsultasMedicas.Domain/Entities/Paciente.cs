using ConsultasMedicas.Domain.Common;

namespace ConsultasMedicas.Domain.Entities;

public class Paciente : AggregateRoot
{
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Email { get; private set; }
    public string Telefono { get; private set; } // Opcional
    public DateTime FechaNacimiento { get; private set; }
    // public Guid? IdHistorialClinico { get; private set; } // Referencia simple

    // Constructor privado para EF o Mapeadores
    private Paciente() : base()
    {
        Nombre = string.Empty;
        Apellido = string.Empty;
        Email = string.Empty;
        Telefono = string.Empty;
    }

    // Constructor para crear un nuevo Paciente (Factory Method)
    public static Paciente Crear(string nombre, string apellido, string email, DateTime fechaNacimiento,
        string? telefono = null)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) ||
            string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Nombre, apellido y email son requeridos.");
        // Aquí podrían ir más validaciones (formato email, etc.)

        return new Paciente(Guid.NewGuid())
        {
            Nombre = nombre,
            Apellido = apellido,
            Email = email,
            FechaNacimiento = fechaNacimiento,
            Telefono = telefono ?? string.Empty // Asegurar que no sea null
        };
    }

    // Métodos para modificar el estado (ejemplo)
    public void ActualizarContacto(string email, string? telefono)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email es requerido.");
        Email = email;
        Telefono = telefono ?? string.Empty;
    }

    // Constructor protegido para la creación desde el factory method y posible herencia
    protected Paciente(Guid id) : base(id)
    {
        Nombre = string.Empty;
        Apellido = string.Empty;
        Email = string.Empty;
        Telefono = string.Empty;
    }
}