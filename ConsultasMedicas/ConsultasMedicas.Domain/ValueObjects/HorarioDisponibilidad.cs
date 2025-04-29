namespace ConsultasMedicas.Domain.ValueObjects;

// Objeto de Valor para representar un bloque de disponibilidad
// (Similar al existente, usando record para inmutabilidad)
public record HorarioDisponibilidad(DayOfWeek DiaSemana, TimeOnly HoraInicio, TimeOnly HoraFin)
{
    public static HorarioDisponibilidad Crear(DayOfWeek dia, TimeOnly inicio, TimeOnly fin)
    {
        if (inicio >= fin) throw new ArgumentException("La hora de inicio debe ser anterior a la hora de fin.");
        // Otras validaciones si son necesarias
        return new HorarioDisponibilidad(dia, inicio, fin);
    }
}