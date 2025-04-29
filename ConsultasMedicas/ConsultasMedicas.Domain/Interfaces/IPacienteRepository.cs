using ConsultasMedicas.Domain.Entities;

// Necesario para Paciente

namespace ConsultasMedicas.Domain.Interfaces;

public interface IPacienteRepository
{
    Task<Paciente?> GetByIdAsync(Guid id);
    Task<IEnumerable<Paciente>> GetAllAsync();
    Task AddAsync(Paciente paciente);
    Task UpdateAsync(Paciente paciente); // Necesario para guardar cambios
    Task DeleteAsync(Guid id); // Opcional
}