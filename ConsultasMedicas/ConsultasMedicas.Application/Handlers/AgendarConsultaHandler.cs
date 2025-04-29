using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Interfaces;
using ConsultasMedicas.Domain.ValueObjects;
using MediatR;

// Necesario para AgendarConsultaCommand

namespace ConsultasMedicas.Application.Handlers;

public class AgendarConsultaHandler : IRequestHandler<AgendarConsultaCommand, bool>
{
    private readonly IConsultaRepository _consultaRepository;
    private readonly IMedicoRepository _medicoRepository;

    public AgendarConsultaHandler(IConsultaRepository consultaRepository, IMedicoRepository medicoRepository)
    {
        _consultaRepository = consultaRepository;
        _medicoRepository = medicoRepository;
    }

    public async Task<bool> Handle(AgendarConsultaCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la consulta y el médico
        var consulta = await _consultaRepository.GetByIdAsync(request.ConsultaId);
        if (consulta == null) throw new KeyNotFoundException($"Consulta {request.ConsultaId} no encontrada.");

        var medico = await _medicoRepository.GetByIdAsync(request.MedicoId);
        if (medico == null) throw new KeyNotFoundException($"Médico {request.MedicoId} no encontrado.");

        // 2. Crear el VO HorarioConsulta
        var horario = HorarioConsulta.Crear(request.FechaHoraInicio, request.FechaHoraFin);

        // 3. Llamar al método del agregado para agendar (la lógica de dominio está aquí)
        try
        {
            // Pasamos el objeto Medico completo para que Consulta pueda validar disponibilidad y especialidad
            consulta.Agendar(request.MedicoId, horario, medico);
        }
        catch (InvalidOperationException ex)
        {
            // Manejar errores específicos de dominio (ej. médico no disponible, especialidad incorrecta)
            // Podríamos loggear el error y devolver false o lanzar una excepción específica de aplicación.
            Console.WriteLine($"Error al agendar consulta {request.ConsultaId}: {ex.Message}"); // Log simple
            return false; // Indicar fallo
            // O lanzar una ApplicationException: throw new ApplicationException($"Error al agendar: {ex.Message}", ex);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(
                $"Argumento inválido al agendar consulta {request.ConsultaId}: {ex.Message}"); // Log simple
            return false; // Indicar fallo
        }


        // 4. Guardar los cambios en la consulta
        await _consultaRepository.UpdateAsync(consulta);

        // (Opcional) Aquí se podrían disparar eventos de dominio si Consulta los generó,
        // y tener handlers de eventos para enviar notificaciones, etc.

        return true; // Indicar éxito
    }
}