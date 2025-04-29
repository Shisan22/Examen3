using MediatR;

namespace ConsultasMedicas.Application.Commands;

// Comando para crear un nuevo médico
public record CrearMedicoCommand(
    string Nombre,
    string Apellido,
    string EspecialidadNombre,
    string EspecialidadCodigo
) : IRequest<Guid>; // Devuelve el ID del médico creado