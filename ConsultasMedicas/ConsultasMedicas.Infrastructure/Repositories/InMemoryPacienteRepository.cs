using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;

namespace ConsultasMedicas.Infrastructure.Repositories;

// Implementación simple de Repositorio en Memoria para Pacientes
public class InMemoryPacienteRepository : IPacienteRepository
{
    // Usar ConcurrentDictionary para seguridad en concurrencia si la API maneja múltiples requests
    private static readonly Dictionary<Guid, Paciente> _pacientes = new();

    public Task<Paciente?> GetByIdAsync(Guid id)
    {
        _pacientes.TryGetValue(id, out var paciente);
        // Devolver una copia para simular desapego de la "BD", aunque con records/VOs inmutables es menos problemático
        // return Task.FromResult(paciente?.Clone()); // Necesitaría un método Clone en Paciente
        return Task.FromResult(paciente); // Simplificado por ahora
    }

    public Task<IEnumerable<Paciente>> GetAllAsync()
    {
        return Task.FromResult(_pacientes.Values.AsEnumerable()); //.Select(p => p.Clone()));
    }

    public Task AddAsync(Paciente paciente)
    {
        ArgumentNullException.ThrowIfNull(paciente);
        _pacientes[paciente.Id] = paciente; // Agrega o reemplaza si ya existe (comportamiento de Diccionario)
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Paciente paciente)
    {
        ArgumentNullException.ThrowIfNull(paciente);
        if (!_pacientes.ContainsKey(paciente.Id))
            // O lanzar excepción o manejar como "upsert"
            throw new KeyNotFoundException($"Paciente con ID {paciente.Id} no encontrado para actualizar.");
        _pacientes[paciente.Id] = paciente; // Reemplaza la instancia existente
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _pacientes.Remove(id);
        return Task.CompletedTask;
    }
}