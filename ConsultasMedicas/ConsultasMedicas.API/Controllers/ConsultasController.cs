using ConsultasMedicas.Application.Commands;
using ConsultasMedicas.Domain.Entities;
using ConsultasMedicas.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
// Para IEnumerable
// Para Task
// Para StatusCodes
// Necesario para inyectar IMediator
// Para los DTOs de entrada si fueran necesarios aquí
// Para IConsultaRepository

// Para devolver Consulta (o DTO)

namespace ConsultasMedicas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConsultaRepository _consultaRepository; // Para Queries simples (opcional)

    // Inyectar MediatR y opcionalmente repositorios para lectura directa
    public ConsultasController(IMediator mediator, IConsultaRepository consultaRepository)
    {
        _mediator = mediator;
        _consultaRepository = consultaRepository;
    }

    // POST api/consultas/solicitar
    [HttpPost("solicitar")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Si el paciente no existe
    public async Task<IActionResult> SolicitarConsulta([FromBody] SolicitarConsultaCommand command)
    {
        if (!ModelState.IsValid) // Validación básica de atributos del DTO/Command
            return BadRequest(ModelState);

        try
        {
            var consultaId = await _mediator.Send(command);
            // Devolver la ruta al nuevo recurso creado
            return CreatedAtAction(nameof(GetConsultaById), new { id = consultaId }, consultaId);
        }
        catch (KeyNotFoundException ex) // Capturar excepción si el paciente no se encuentra
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex) // Capturar errores de validación de VOs o creación
        {
            return BadRequest(new { message = ex.Message });
        }
        // Capturar otras excepciones si es necesario
    }

    // POST api/consultas/agendar
    [HttpPost("agendar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AgendarConsulta([FromBody] AgendarConsultaCommand command)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var exito = await _mediator.Send(command);
            if (exito)
                return Ok(new { message = "Consulta agendada exitosamente." });
            else
                // Si el handler devuelve false, podría ser por una regla de negocio no cumplida (ej. no disponible)
                // Devolvemos BadRequest o un código más específico si es posible.
                return BadRequest(new
                {
                    message = "No se pudo agendar la consulta. Verifique la disponibilidad y especialidad del médico."
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex) // Errores de estado o reglas de negocio del agregado
        {
            return BadRequest(new { message = $"Error de negocio: {ex.Message}" });
        }
        catch (ArgumentException ex) // Errores en datos de entrada (ej. horario inválido)
        {
            return BadRequest(new { message = $"Datos inválidos: {ex.Message}" });
        }
        // catch (ApplicationException ex) // Capturar excepciones específicas de la capa de aplicación
        // {
        //     return BadRequest(new { message = ex.Message });
        // }
    }


    // GET api/consultas/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Consulta),
        StatusCodes.Status200OK)] // Devolver el objeto Consulta directamente (o un DTO de lectura)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConsultaById(Guid id)
    {
        // Para lecturas simples, podemos usar el repositorio directamente
        var consulta = await _consultaRepository.GetByIdAsync(id);

        if (consulta == null) return NotFound();

        // Aquí deberías devolver un DTO de lectura (ConsultaDto) en lugar del agregado Consulta directamente
        // para evitar exponer el modelo de dominio. Por simplicidad, lo devuelvo directo.
        return Ok(consulta);
    }

    // GET api/consultas/paciente/{pacienteId}
    [HttpGet("paciente/{pacienteId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Consulta>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConsultasByPaciente(Guid pacienteId)
    {
        var consultas = await _consultaRepository.GetByPacienteIdAsync(pacienteId);
        // De nuevo, idealmente mapear a DTOs de lectura
        return Ok(consultas);
    }

    // POST api/consultas/{id}/realizar
    [HttpPost("{id:guid}/realizar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarcarRealizada(Guid id, [FromBody] MarcarConsultaRealizadaCommand command)
    {
        if (id != command.ConsultaId)
            return BadRequest("El ID de la consulta en la ruta no coincide con el del cuerpo.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var exito = await _mediator.Send(command);
            if (exito) return Ok(new { message = "Consulta marcada como realizada exitosamente." });
            else
                return BadRequest(new
                {
                    message =
                        "No se pudo marcar la consulta como realizada. Verifique el estado actual y los datos proporcionados."
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = $"Error de negocio: {ex.Message}" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = $"Datos inválidos: {ex.Message}" });
        }
    }

    // POST api/consultas/{id}/cancelar
    [HttpPost("{consultaId:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelarConsulta(Guid consultaId, [FromBody] CancelarConsultaCommand command)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var nuevoCommand = command with { ConsultaId = consultaId };
            var exito = await _mediator.Send(nuevoCommand);
            if (exito)
                return Ok(new { message = "Consulta cancelada exitosamente." });
            else
                return BadRequest(new { message = "No se pudo cancelar la consulta." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = $"Error de negocio: {ex.Message}" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = $"Datos inválidos: {ex.Message}" });
        }
    }

    // POST api/consultas/{id}/prescripciones
    [HttpPost("{consultaId:guid}/prescripciones")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AgregarPrescripcion(Guid consultaId, [FromBody] AgregarPrescripcionCommand command)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var nuevoCommand = command with { ConsultaId = consultaId };
            var exito = await _mediator.Send(nuevoCommand);
            if (exito) return Ok(new { message = "Prescripción agregada exitosamente." });
            else
                return BadRequest(new
                {
                    message =
                        "No se pudo agregar la prescripción. Verifique que la consulta esté realizada y los datos sean correctos."
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = $"Error de negocio: {ex.Message}" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = $"Datos inválidos: {ex.Message}" });
        }
    }
}