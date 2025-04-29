namespace ConsultasMedicas.Domain.ValueObjects;

// Objeto de Valor para representar la Especialidad Médica
public record Especialidad(string Nombre, string Codigo)
{
    // Validaciones pueden ir aquí
    public static Especialidad Crear(string nombre, string codigo)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("Nombre y código de especialidad son requeridos.");
        return new Especialidad(nombre, codigo);
    }
}