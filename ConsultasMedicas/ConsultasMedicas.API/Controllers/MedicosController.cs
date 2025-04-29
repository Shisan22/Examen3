using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// Para devolver Medico (o DTO)

namespace ConsultasMedicas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMedicoRepository _medicoRepository; // Para queries

    public MedicosController(IMediator mediator, IMedicoRepository medicoRepository)
    {
        _mediator = mediator;
        _medicoRepository = medicoRepository;
    }

    // POST api/medicos
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearMedico([FromBody] CrearMedicoCommand command)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var medicoId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMedicoById), new { id = medicoId }, medicoId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST api/medicos/{medicoId}/disponibilidad
    [HttpPost("{medicoId:guid}/disponibilidad")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AgregarDisponibilidad(Guid medicoId,
        [FromBody] AgregarDisponibilidadCommand command)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            // Crear un nuevo comando con el ID de la URL
            var nuevoCommand = command with { MedicoId = medicoId };
            var exito = await _mediator.Send(nuevoCommand);
            if (exito)
                return Ok(new { message = "Disponibilidad agregada exitosamente." });
            else
                return BadRequest(new
                    { message = "No se pudo agregar la disponibilidad. Verifique los horarios existentes." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message }); // Médico no encontrado
        }
        catch (ArgumentException ex) // Error en la creación del VO HorarioDisponibilidad
        {
            return BadRequest(new { message = $"Datos de horario inválidos: {ex.Message}" });
        }
        // catch (ApplicationException ex) // Si el handler lanzara excepciones de aplicación
        // {
        //     return BadRequest(new { message = ex.Message });
        // }
    }


    // GET api/medicos/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Medico), StatusCodes.Status200OK)] // Idealmente MedicoDto
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMedicoById(Guid id)
    {
        var medico = await _medicoRepository.GetByIdAsync(id);
        if (medico == null) return NotFound();
        // Mapear a DTO de lectura
        return Ok(medico);
    }

    // GET api/medicos
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Medico>), StatusCodes.Status200OK)] // Idealmente IEnumerable<MedicoDto>
    public async Task<IActionResult> GetAllMedicos()
    {
        var medicos = await _medicoRepository.GetAllAsync();
        // Mapear a DTOs de lectura
        return Ok(medicos);
    }
}