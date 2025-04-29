using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;
using ConsultasMedicas.Domain.ValueObjects;
using MediatR;

// Necesario para SolicitarConsultaCommand

namespace ConsultasMedicas.Application.Handlers;

// Handler para el comando SolicitarConsultaCommand
public class SolicitarConsultaHandler : IRequestHandler<SolicitarConsultaCommand, Guid>
{
    private readonly IConsultaRepository _consultaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    // Podríamos inyectar IUnitOfWork si lo usáramos

    public SolicitarConsultaHandler(IConsultaRepository consultaRepository, IPacienteRepository pacienteRepository)
    {
        _consultaRepository = consultaRepository;
        _pacienteRepository = pacienteRepository;
    }

    public async Task<Guid> Handle(SolicitarConsultaCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que el paciente existe
        var paciente = await _pacienteRepository.GetByIdAsync(request.PacienteId);
        if (paciente == null) throw new KeyNotFoundException($"Paciente con ID {request.PacienteId} no encontrado.");
        // O usar un resultado más elegante (ej. FluentResults, OneOf)
        // 2. Crear el objeto de valor Especialidad si se proporcionó
        Especialidad? especialidad = null;
        if (!string.IsNullOrWhiteSpace(request.EspecialidadNombre) &&
            !string.IsNullOrWhiteSpace(request.EspecialidadCodigo))
            // Aquí podríamos validar si la especialidad existe en algún catálogo,
            // pero para simplificar, la creamos directamente.
            especialidad = Especialidad.Crear(request.EspecialidadNombre, request.EspecialidadCodigo);

        // 3. Crear el agregado Consulta usando el Factory Method
        var nuevaConsulta = Consulta.Solicitar(
            request.PacienteId,
            request.MotivoConsulta,
            especialidad
        );

        // 4. Persistir el nuevo agregado
        await _consultaRepository.AddAsync(nuevaConsulta);

        // (Opcional) Si usáramos UnitOfWork: await _unitOfWork.CompleteAsync();

        // 5. Devolver el ID de la nueva consulta
        return nuevaConsulta.Id;
    }
}