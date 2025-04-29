using MediatR;

namespace ConsultasMedicas.Application.Commands;

public record MarcarConsultaRealizadaCommand(Guid ConsultaId, string NotasMedico) : IRequest<bool>;