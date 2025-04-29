namespace ConsultasMedicas.Application.DTOs;

// DTO para añadir disponibilidad a un médico
public record AgregarDisponibilidadDto(Guid MedicoId, DayOfWeek DiaSemana, TimeOnly HoraInicio, TimeOnly HoraFin);