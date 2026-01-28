using System.ComponentModel.DataAnnotations;

namespace PowerMas.Api.Contracts;

/// <summary>
/// DTO para crear/actualizar Beneficiario
/// </summary>
public class BeneficiarioRequest
{
    [Required(ErrorMessage = "Nombres es requerido")]
    [MaxLength(100, ErrorMessage = "Nombres no puede exceder 100 caracteres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Apellidos es requerido")]
    [MaxLength(100, ErrorMessage = "Apellidos no puede exceder 100 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "DocumentoIdentidadId es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "DocumentoIdentidadId debe ser mayor a 0")]
    public int DocumentoIdentidadId { get; set; }

    [Required(ErrorMessage = "NumeroDocumento es requerido")]
    [MaxLength(20, ErrorMessage = "NumeroDocumento no puede exceder 20 caracteres")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "FechaNacimiento es requerida")]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Sexo es requerido")]
    [RegularExpression("^[MF]$", ErrorMessage = "Sexo debe ser 'M' o 'F'")]
    public string Sexo { get; set; } = string.Empty;
}
