using PowerMas.Api.Domain;

namespace PowerMas.Api.Services;

/// <summary>
/// Servicio para DocumentoIdentidad
/// </summary>
public interface IDocumentoIdentidadService
{
    Task<IEnumerable<DocumentoIdentidad>> ListarActivosAsync();
}
