namespace ConsultasMedicas.Domain.ValueObjects;

// Objeto de Valor para el horario específico de una consulta
public record HorarioConsulta(DateTime FechaHoraInicio, DateTime FechaHoraFin)
{
    public static HorarioConsulta Crear(DateTime inicio, DateTime fin)
    {
        if (inicio >= fin)
            throw new ArgumentException("La fecha/hora de inicio debe ser anterior a la fecha/hora de fin.");
        // Podría tener validación de duración mínima/máxima si es una regla de negocio
        return new HorarioConsulta(inicio, fin);
    }
}