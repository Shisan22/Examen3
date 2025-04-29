using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;
using ConsultasMedicas.Domain.ValueObjects;
using MediatR;

namespace ConsultasMedicas.Application.Handlers;

public class CrearMedicoHandler : IRequestHandler<CrearMedicoCommand, Guid>
{
    private readonly IMedicoRepository _medicoRepository;

    public CrearMedicoHandler(IMedicoRepository medicoRepository)
    {
        _medicoRepository = medicoRepository;
    }

    public async Task<Guid> Handle(CrearMedicoCommand request, CancellationToken cancellationToken)
    {
        // Crear VO Especialidad
        var especialidad = Especialidad.Crear(request.EspecialidadNombre, request.EspecialidadCodigo);

        // Crear Agregado Medico
        var nuevoMedico = Medico.Crear(
            request.Nombre,
            request.Apellido,
            especialidad
        );

        // Persistir
        await _medicoRepository.AddAsync(nuevoMedico);

        // Devolver ID
        return nuevoMedico.Id;
    }
}