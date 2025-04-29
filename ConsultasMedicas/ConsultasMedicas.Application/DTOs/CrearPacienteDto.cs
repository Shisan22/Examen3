namespace ConsultasMedicas.Application.DTOs;

// DTO para crear un paciente (similar al existente)
public record CrearPacienteDto(
    string Nombre,
    string Apellido,
    string Email,
    DateTime FechaNacimiento,
    string? Telefono);