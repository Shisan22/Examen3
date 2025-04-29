using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;
using MediatR;

namespace ConsultasMedicas.Application.Handlers;

public class CrearPacienteHandler : IRequestHandler<CrearPacienteCommand, Guid>
{
    private readonly IPacienteRepository _pacienteRepository;

    public CrearPacienteHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<Guid> Handle(CrearPacienteCommand request, CancellationToken cancellationToken)
    {
        // Validaciones adicionales podrían ir aquí (ej. formato email, si ya existe, etc.)

        // Crear el agregado Paciente usando el Factory Method
        var nuevoPaciente = Paciente.Crear(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.FechaNacimiento,
            request.Telefono
        );

        // Persistir
        await _pacienteRepository.AddAsync(nuevoPaciente);

        // Devolver ID
        return nuevoPaciente.Id;
    }
}