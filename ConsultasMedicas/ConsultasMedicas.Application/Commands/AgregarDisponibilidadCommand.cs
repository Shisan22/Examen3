using MediatR;

namespace ConsultasMedicas.Application.Commands;

// Comando para agregar un bloque de disponibilidad a un médico
public record AgregarDisponibilidadCommand(
    Guid MedicoId,
    DayOfWeek DiaSemana,
    TimeOnly HoraInicio,
    TimeOnly HoraFin
) : IRequest<bool>; // Devuelve true si se agregó, false o excepción si no