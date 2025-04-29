using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Interfaces;
using MediatR;

namespace ConsultasMedicas.Application.Handlers;

public class CancelarConsultaHandler : IRequestHandler<CancelarConsultaCommand, bool>
{
    private readonly IConsultaRepository _consultaRepository;

    public CancelarConsultaHandler(IConsultaRepository consultaRepository)
    {
        _consultaRepository = consultaRepository;
    }

    public async Task<bool> Handle(CancelarConsultaCommand request, CancellationToken cancellationToken)
    {
        var consulta = await _consultaRepository.GetByIdAsync(request.ConsultaId);
        if (consulta == null) throw new KeyNotFoundException($"Consulta {request.ConsultaId} no encontrada.");
        try
        {
            consulta.Cancelar(request.Motivo);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error al cancelar consulta {request.ConsultaId}: {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Argumento inválido al cancelar consulta {request.ConsultaId}: {ex.Message}");
            return false;
        }

        await _consultaRepository.UpdateAsync(consulta);
        return true;
    }
}