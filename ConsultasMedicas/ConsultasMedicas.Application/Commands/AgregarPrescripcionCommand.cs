using MediatR;

namespace ConsultasMedicas.Application.Commands;

public record AgregarPrescripcionCommand(Guid ConsultaId, string Medicamento, string Dosis, string Instrucciones)
    : IRequest<bool>;