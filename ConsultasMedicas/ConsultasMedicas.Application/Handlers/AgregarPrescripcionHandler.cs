using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Interfaces;
using MediatR;

namespace ConsultasMedicas.Application.Handlers;

public class AgregarPrescripcionHandler : IRequestHandler<AgregarPrescripcionCommand, bool>
{
    private readonly IConsultaRepository _consultaRepository;

    public AgregarPrescripcionHandler(IConsultaRepository consultaRepository)
    {
        _consultaRepository = consultaRepository;
    }

    public async Task<bool> Handle(AgregarPrescripcionCommand request, CancellationToken cancellationToken)
    {
        var consulta = await _consultaRepository.GetByIdAsync(request.ConsultaId);
        if (consulta == null) throw new KeyNotFoundException($"Consulta {request.ConsultaId} no encontrada.");
        try
        {
            consulta.AgregarPrescripcion(request.Medicamento, request.Dosis, request.Instrucciones);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error al agregar prescripción a consulta {request.ConsultaId}: {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(
                $"Argumento inválido al agregar prescripción a consulta {request.ConsultaId}: {ex.Message}");
            return false;
        }

        await _consultaRepository.UpdateAsync(consulta);
        return true;
    }
}