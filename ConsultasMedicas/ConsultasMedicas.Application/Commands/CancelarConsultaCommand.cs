using MediatR;

namespace ConsultasMedicas.Application.Commands;

public record CancelarConsultaCommand(Guid ConsultaId, string Motivo) : IRequest<bool>;