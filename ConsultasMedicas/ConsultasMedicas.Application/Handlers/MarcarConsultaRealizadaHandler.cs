using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Interfaces;
using MediatR;

namespace ConsultasMedicas.Application.Handlers;

public class MarcarConsultaRealizadaHandler : IRequestHandler<MarcarConsultaRealizadaCommand, bool>
{
    private readonly IConsultaRepository _consultaRepository;

    public MarcarConsultaRealizadaHandler(IConsultaRepository consultaRepository)
    {
        _consultaRepository = consultaRepository;
    }

    public async Task<bool> Handle(MarcarConsultaRealizadaCommand request, CancellationToken cancellationToken)
    {
        var consulta = await _consultaRepository.GetByIdAsync(request.ConsultaId);
        if (consulta == null) throw new KeyNotFoundException($"Consulta {request.ConsultaId} no encontrada.");
        try
        {
            consulta.MarcarRealizada(request.NotasMedico);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error al marcar consulta {request.ConsultaId} como realizada: {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(
                $"Argumento inválido al marcar consulta {request.ConsultaId} como realizada: {ex.Message}");
            return false;
        }

        await _consultaRepository.UpdateAsync(consulta);
        return true;
    }
}