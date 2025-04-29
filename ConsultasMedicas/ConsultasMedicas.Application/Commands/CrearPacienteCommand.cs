using MediatR;

// Para usar CrearPacienteDto indirectamente

namespace ConsultasMedicas.Application.Commands;

// Comando para crear un nuevo paciente
// Usamos el DTO directamente como comando por simplicidad en este caso
public record CrearPacienteCommand(
    string Nombre,
    string Apellido,
    string Email,
    DateTime FechaNacimiento,
    string? Telefono
) : IRequest<Guid>; // Devuelve el ID del paciente creado