namespace PowerMas.Api.Domain;

/// <summary>
/// Entidad Beneficiario
/// </summary>
public class Beneficiario
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int DocumentoIdentidadId { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public char Sexo { get; set; }
}
