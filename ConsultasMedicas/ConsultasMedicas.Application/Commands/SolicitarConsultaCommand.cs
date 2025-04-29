using MediatR;

namespace ConsultasMedicas.Application.Commands;

// Comando para solicitar una nueva consulta
public record SolicitarConsultaCommand(
    Guid PacienteId,
    string MotivoConsulta,
    string? EspecialidadNombre, // Opcional, pasamos datos primitivos
    string? EspecialidadCodigo // Opcional
) : IRequest<Guid>; // Devuelve el ID de la consulta creada