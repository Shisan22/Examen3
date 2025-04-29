using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.ValueObjects;
// Necesario para Medico

// Necesario para Especialidad

namespace ConsultasMedicas.Domain.Interfaces;

public interface IMedicoRepository
{
    Task<Medico?> GetByIdAsync(Guid id);

    Task<IEnumerable<Medico>> GetAllAsync();

    // Método específico útil para la asignación
    Task<IEnumerable<Medico>> FindByEspecialidadAsync(Especialidad especialidad);

    Task<IEnumerable<Medico>>
        FindDisponiblesAsync(DateTime fechaHora, Especialidad? especialidad = null); // Lógica más compleja

    Task AddAsync(Medico medico);
    Task UpdateAsync(Medico medico);
    Task DeleteAsync(Guid id);
}