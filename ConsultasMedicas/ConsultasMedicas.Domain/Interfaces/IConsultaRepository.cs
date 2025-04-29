using ConsultasMedicas.Domain.Entities;

// Necesario para Consulta

namespace ConsultasMedicas.Domain.Interfaces;

public interface IConsultaRepository
{
    Task<Consulta?> GetByIdAsync(Guid id);
    Task<IEnumerable<Consulta>> GetAllAsync();
    Task<IEnumerable<Consulta>> GetByPacienteIdAsync(Guid pacienteId);
    Task<IEnumerable<Consulta>> GetByMedicoIdAsync(Guid medicoId);
    Task AddAsync(Consulta consulta);
    Task UpdateAsync(Consulta consulta);
    Task DeleteAsync(Guid id);
}