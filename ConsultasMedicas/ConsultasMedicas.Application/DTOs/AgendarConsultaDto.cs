namespace ConsultasMedicas.Application.DTOs;

// DTO para agendar una consulta (necesita ID de consulta, médico y horario)
public record AgendarConsultaDto(Guid ConsultaId, Guid MedicoId, DateTime FechaHoraInicio, DateTime FechaHoraFin);