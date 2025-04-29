namespace ConsultasMedicas.Application.DTOs;

// DTO para crear un médico (similar al existente)
public record CrearMedicoDto(string Nombre, string Apellido, string EspecialidadNombre, string EspecialidadCodigo);