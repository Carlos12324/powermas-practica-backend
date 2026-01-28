namespace PowerMas.Api.Domain;

/// <summary>
/// Beneficiario con datos del documento (para listado)
/// </summary>
public class BeneficiarioDetalle
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int DocumentoIdentidadId { get; set; }
    public string DocumentoNombre { get; set; } = string.Empty;
    public string DocumentoAbreviatura { get; set; } = string.Empty;
    public string DocumentoPais { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public char Sexo { get; set; }
}
