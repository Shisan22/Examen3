using ConsultasMedicas.Domain.Common;
using ConsultasMedicas.Domain.ValueObjects;

namespace ConsultasMedicas.Domain.Entities;

public class Medico : AggregateRoot
{
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public Especialidad? Especialidad { get; private set; } // VO
    public List<HorarioDisponibilidad> Disponibilidad { get; private set; } // Colección de VOs

    private Medico() : base()
    {
        Nombre = string.Empty;
        Apellido = string.Empty;
        Especialidad = null;
        Disponibilidad = new List<HorarioDisponibilidad>();
    }

    // Factory Method
    public static Medico Crear(string nombre, string apellido, Especialidad especialidad)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("Nombre y apellido son requeridos.");
        ArgumentNullException.ThrowIfNull(especialidad);

        return new Medico(Guid.NewGuid())
        {
            Nombre = nombre,
            Apellido = apellido,
            Especialidad = especialidad,
            Disponibilidad = new List<HorarioDisponibilidad>() // Inicializar lista
        };
    }

    // Constructor protegido
    protected Medico(Guid id) : base(id)
    {
        Nombre = string.Empty;
        Apellido = string.Empty;
        Especialidad = null;
        Disponibilidad = new List<HorarioDisponibilidad>();
    }

    public void AgregarDisponibilidad(HorarioDisponibilidad horario)
    {
        ArgumentNullException.ThrowIfNull(horario);
        // Validación para evitar solapamientos (lógica de dominio importante)
        if (Disponibilidad.Any(h => h.DiaSemana == horario.DiaSemana && HorariosSolapados(h, horario)))
            throw new InvalidOperationException("El nuevo horario se solapa con uno existente.");
        Disponibilidad.Add(horario);
        // AddDomainEvent(new DisponibilidadMedicoActualizadaEvent(this.Id, horario));
    }

    public void RemoverDisponibilidad(HorarioDisponibilidad horario)
    {
        ArgumentNullException.ThrowIfNull(horario);
        Disponibilidad
            .Remove(horario); // Asume que HorarioDisponibilidad implementa igualdad correctamente (record lo hace)
    }

    // Método simple para verificar solapamiento (puede necesitar ser más robusto)
    private bool HorariosSolapados(HorarioDisponibilidad h1, HorarioDisponibilidad h2)
    {
        // Verifica si h2 inicia antes de que h1 termine Y h2 termina después de que h1 inicie
        return h2.HoraInicio < h1.HoraFin && h2.HoraFin > h1.HoraInicio;
    }

    // Método para verificar si el médico está disponible en una fecha/hora específica
    public bool EstaDisponible(DateTime fechaHora)
    {
        var fechaHoraComparar = fechaHora.Kind == DateTimeKind.Utc
            ? fechaHora.ToLocalTime() // Convierte de UTC a Local
            : fechaHora; // Usar directamente si ya es Local o Unspecified

        var diaSemana = fechaHoraComparar.DayOfWeek;
        // Extraer la hora de la fecha/hora (potencialmente convertida a local)
        var hora = TimeOnly.FromDateTime(fechaHoraComparar);

        // La lógica de comparación permanece igual
        return Disponibilidad.Any(h => h.DiaSemana == diaSemana && hora >= h.HoraInicio && hora < h.HoraFin);
    }
}