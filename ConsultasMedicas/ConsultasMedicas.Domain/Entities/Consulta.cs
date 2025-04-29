using ConsultasMedicas.Domain.Common;
using ConsultasMedicas.Domain.ValueObjects;

namespace ConsultasMedicas.Domain.Entities;

public record DetallePrescripcion(string Medicamento, string Dosis, string Instrucciones);

// Entidad Consulta (Raíz de Agregado)
public class Consulta : AggregateRoot
{
    public Guid PacienteId { get; private set; }
    public Guid? MedicoId { get; private set; } // Nullable hasta que se asigne
    public Especialidad? EspecialidadRequerida { get; private set; } // Puede ser null si el paciente no especifica
    public HorarioConsulta? Horario { get; private set; } // Nullable hasta que se agende
    public EstadoConsulta Estado { get; private set; }
    public string? MotivoConsulta { get; private set; } // Añadido para contexto
    public string? NotasMedico { get; private set; } // Para después de la consulta
    public List<DetallePrescripcion>? Prescripciones { get; private set; } // VO o Entidad simple
    public string? MotivoCancelacion { get; private set; } // Añadido para cancelación

    private Consulta() : base()
    {
        Prescripciones = new List<DetallePrescripcion>();
    }

    // Factory Method para crear una nueva solicitud de consulta
    public static Consulta Solicitar(Guid pacienteId, string motivoConsulta, Especialidad? especialidadRequerida = null)
    {
        if (pacienteId == Guid.Empty) throw new ArgumentException("ID de paciente inválido.");
        if (string.IsNullOrWhiteSpace(motivoConsulta))
            throw new ArgumentException("El motivo de la consulta es requerido.");

        return new Consulta(Guid.NewGuid())
        {
            PacienteId = pacienteId,
            MotivoConsulta = motivoConsulta,
            EspecialidadRequerida = especialidadRequerida,
            Estado = EstadoConsulta.Solicitada,
            Prescripciones = new List<DetallePrescripcion>()
        };
    }

    // Constructor protegido
    protected Consulta(Guid id) : base(id)
    {
        Prescripciones = new List<DetallePrescripcion>();
    }

    // Método para agendar la consulta
    public void Agendar(Guid medicoId, HorarioConsulta horario, Medico medico)
    {
        if (Estado != EstadoConsulta.Solicitada)
            throw new InvalidOperationException("La consulta solo se puede agendar si está en estado 'Solicitada'.");
        if (medicoId == Guid.Empty) throw new ArgumentException("ID de médico inválido.");
        ArgumentNullException.ThrowIfNull(horario);
        ArgumentNullException.ThrowIfNull(medico);
        if (medico.Id != medicoId)
            throw new ArgumentException("El ID del médico proporcionado no coincide con el objeto médico.");

        // Validar si la especialidad del médico coincide (si se requirió una)
        if (EspecialidadRequerida != null && medico.Especialidad != EspecialidadRequerida)
            throw new InvalidOperationException(
                $"El médico no tiene la especialidad requerida '{EspecialidadRequerida.Nombre}'.");

        // Validar disponibilidad del médico (lógica de dominio dentro del agregado o usando el objeto Medico)
        if (!medico.EstaDisponible(horario.FechaHoraInicio) ||
            !medico.EstaDisponible(horario.FechaHoraFin.AddTicks(-1))) // Verificar inicio y justo antes del fin
            throw new InvalidOperationException("El médico no está disponible en el horario seleccionado.");

        MedicoId = medicoId;
        Horario = horario;
        Estado = EstadoConsulta.Agendada; // Cambia a Agendada

        // AddDomainEvent(new ConsultaAgendadaEvent(this.Id, PacienteId, MedicoId.Value, Horario));
    }

    // Método para marcar como realizada
    public void MarcarRealizada(string notasMedico)
    {
        // Permitir marcar como realizada si está Agendada o Confirmada (si se añade ese estado)
        if (Estado != EstadoConsulta.Agendada && Estado != EstadoConsulta.Confirmada)
            throw new InvalidOperationException(
                "La consulta debe estar agendada (o confirmada) para marcarse como realizada.");
        if (string.IsNullOrWhiteSpace(notasMedico))
            throw new ArgumentException("Las notas del médico son requeridas al completar la consulta.");

        Estado = EstadoConsulta.Realizada;
        NotasMedico = notasMedico;
        // AddDomainEvent(new ConsultaRealizadaEvent(this.Id));
    }

    // Método para cancelar
    public void Cancelar(string motivo)
    {
        // Se puede cancelar si está Solicitada, Agendada o Confirmada
        if (Estado == EstadoConsulta.Realizada || Estado == EstadoConsulta.Cancelada)
            throw new InvalidOperationException("La consulta ya está realizada o cancelada, no se puede cancelar.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Se requiere un motivo para la cancelación.");

        Estado = EstadoConsulta.Cancelada;
        MotivoCancelacion = motivo;
        // AddDomainEvent(new ConsultaCanceladaEvent(this.Id, motivo));
    }

    // Método para añadir prescripción (simplificado)
    public void AgregarPrescripcion(string medicamento, string dosis, string instrucciones)
    {
        // Solo se pueden añadir prescripciones si la consulta está Realizada
        if (Estado != EstadoConsulta.Realizada)
            throw new InvalidOperationException("Solo se pueden añadir prescripciones a consultas realizadas.");
        if (string.IsNullOrWhiteSpace(medicamento) || string.IsNullOrWhiteSpace(dosis) ||
            string.IsNullOrWhiteSpace(instrucciones))
            throw new ArgumentException(
                "Todos los detalles de la prescripción (medicamento, dosis, instrucciones) son requeridos.");

        Prescripciones ??= new List<DetallePrescripcion>(); // Asegurar que la lista exista
        Prescripciones.Add(new DetallePrescripcion(medicamento, dosis, instrucciones));
        // AddDomainEvent(new PrescripcionAgregadaEvent(this.Id, medicamento));
    }
}