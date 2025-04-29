using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Interfaces;
using ConsultasMedicas.Domain.ValueObjects;
using MediatR;

namespace ConsultasMedicas.Application.Handlers;

public class AgregarDisponibilidadHandler : IRequestHandler<AgregarDisponibilidadCommand, bool>
{
    private readonly IMedicoRepository _medicoRepository;

    public AgregarDisponibilidadHandler(IMedicoRepository medicoRepository)
    {
        _medicoRepository = medicoRepository;
    }

    public async Task<bool> Handle(AgregarDisponibilidadCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el médico
        var medico = await _medicoRepository.GetByIdAsync(request.MedicoId);
        if (medico == null) throw new KeyNotFoundException($"Médico con ID {request.MedicoId} no encontrado.");

        // 2. Crear el VO HorarioDisponibilidad
        var nuevoHorario = HorarioDisponibilidad.Crear(request.DiaSemana, request.HoraInicio, request.HoraFin);

        // 3. Llamar al método del agregado para añadir disponibilidad (contiene la lógica de validación)
        try
        {
            medico.AgregarDisponibilidad(nuevoHorario);
        }
        catch (InvalidOperationException ex) // Captura el error de solapamiento
        {
            Console.WriteLine($"Error al agregar disponibilidad para médico {request.MedicoId}: {ex.Message}");
            // Podríamos lanzar una excepción de aplicación o devolver false
            return false;
            // throw new ApplicationException($"No se pudo agregar disponibilidad: {ex.Message}", ex);
        }
        catch (ArgumentException ex) // Captura errores de creación del VO
        {
            Console.WriteLine(
                $"Argumento inválido al agregar disponibilidad para médico {request.MedicoId}: {ex.Message}");
            return false;
        }


        // 4. Guardar los cambios en el médico
        await _medicoRepository.UpdateAsync(medico);

        return true; // Éxito
    }
}