using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;

namespace ConsultasMedicas.Infrastructure.Repositories;

// Implementación simple de Repositorio en Memoria para Consultas
public class InMemoryConsultaRepository : IConsultaRepository
{
    private static readonly Dictionary<Guid, Consulta> _consultas = new();

    public Task<Consulta?> GetByIdAsync(Guid id)
    {
        _consultas.TryGetValue(id, out var consulta);
        return Task.FromResult(consulta);
    }

    public Task<IEnumerable<Consulta>> GetAllAsync()
    {
        return Task.FromResult(_consultas.Values.AsEnumerable());
    }

    public Task<IEnumerable<Consulta>> GetByPacienteIdAsync(Guid pacienteId)
    {
        return Task.FromResult(_consultas.Values.Where(c => c.PacienteId == pacienteId));
    }

    public Task<IEnumerable<Consulta>> GetByMedicoIdAsync(Guid medicoId)
    {
        return Task.FromResult(_consultas.Values.Where(c => c.MedicoId == medicoId));
    }

    public Task AddAsync(Consulta consulta)
    {
        ArgumentNullException.ThrowIfNull(consulta);
        _consultas[consulta.Id] = consulta;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Consulta consulta)
    {
        ArgumentNullException.ThrowIfNull(consulta);
        if (!_consultas.ContainsKey(consulta.Id))
            throw new KeyNotFoundException($"Consulta con ID {consulta.Id} no encontrada.");
        _consultas[consulta.Id] = consulta;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _consultas.Remove(id);
        return Task.CompletedTask;
    }
}