using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// Para devolver Paciente (o DTO)

namespace ConsultasMedicas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPacienteRepository _pacienteRepository; // Para queries

    public PacientesController(IMediator mediator, IPacienteRepository pacienteRepository)
    {
        _mediator = mediator;
        _pacienteRepository = pacienteRepository;
    }

    // POST api/pacientes
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearPaciente([FromBody] CrearPacienteCommand command)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var pacienteId = await _mediator.Send(command);
            // Devolver la ruta al nuevo recurso
            return CreatedAtAction(nameof(GetPacienteById), new { id = pacienteId }, pacienteId);
        }
        catch (ArgumentException ex) // Capturar errores de validación del dominio
        {
            return BadRequest(new { message = ex.Message });
        }
        // Capturar otras excepciones si es necesario
    }

    // GET api/pacientes/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Paciente), StatusCodes.Status200OK)] // Idealmente un PacienteDto
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPacienteById(Guid id)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(id);
        if (paciente == null) return NotFound();
        // Mapear a DTO de lectura si se implementa
        return Ok(paciente);
    }

    // GET api/pacientes
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Paciente>),
        StatusCodes.Status200OK)] // Idealmente IEnumerable<PacienteDto>
    public async Task<IActionResult> GetAllPacientes()
    {
        var pacientes = await _pacienteRepository.GetAllAsync();
        // Mapear a DTOs de lectura si se implementa
        return Ok(pacientes);
    }
}