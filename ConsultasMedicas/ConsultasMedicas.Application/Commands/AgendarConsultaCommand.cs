using MediatR;

namespace ConsultasMedicas.Application.Commands;

// --- Handler para Agendar Consulta (Ejemplo) ---
public record AgendarConsultaCommand(Guid ConsultaId, Guid MedicoId, DateTime FechaHoraInicio, DateTime FechaHoraFin)
    : IRequest<bool>;