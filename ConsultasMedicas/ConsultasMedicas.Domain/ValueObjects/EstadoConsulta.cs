namespace ConsultasMedicas.Domain.ValueObjects;

// Enum para el estado de la consulta
public enum EstadoConsulta
{
    Solicitada = 0,
    Agendada = 1,
    Confirmada = 2, //  Opcional si se necesita confirmación explícita
    Realizada = 3,
    Cancelada = 4
}