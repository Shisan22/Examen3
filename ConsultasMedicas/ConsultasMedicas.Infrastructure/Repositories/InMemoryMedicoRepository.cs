using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;
using ConsultasMedicas.Domain.ValueObjects;

namespace ConsultasMedicas.Infrastructure.Repositories;

// Implementación simple de Repositorio en Memoria para Médicos
public class InMemoryMedicoRepository : IMedicoRepository
{
    private static readonly Dictionary<Guid, Medico> _medicos = new();

    public Task<Medico?> GetByIdAsync(Guid id)
    {
        _medicos.TryGetValue(id, out var medico);
        return Task.FromResult(medico);
    }

    public Task<IEnumerable<Medico>> GetAllAsync()
    {
        return Task.FromResult(_medicos.Values.AsEnumerable());
    }

    public Task<IEnumerable<Medico>> FindByEspecialidadAsync(Especialidad especialidad)
    {
        ArgumentNullException.ThrowIfNull(especialidad);
        return Task.FromResult(_medicos.Values.Where(m => m.Especialidad == especialidad));
    }

    // Implementación simple de búsqueda de disponibilidad
    public Task<IEnumerable<Medico>> FindDisponiblesAsync(DateTime fechaHora, Especialidad? especialidad = null)
    {
        var disponibles = _medicos.Values
            .Where(m => (especialidad == null || m.Especialidad == especialidad) && m.EstaDisponible(fechaHora));
        return Task.FromResult(disponibles);
    }


    public Task AddAsync(Medico medico)
    {
        ArgumentNullException.ThrowIfNull(medico);
        _medicos[medico.Id] = medico;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Medico medico)
    {
        ArgumentNullException.ThrowIfNull(medico);
        if (!_medicos.ContainsKey(medico.Id))
            throw new KeyNotFoundException($"Médico con ID {medico.Id} no encontrado.");
        _medicos[medico.Id] = medico;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _medicos.Remove(id);
        return Task.CompletedTask;
    }
}