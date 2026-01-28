using PowerMas.Api.Domain;

namespace PowerMas.Api.Data;

/// <summary>
/// Repositorio para DocumentoIdentidad
/// </summary>
public interface IDocumentoIdentidadRepository
{
    Task<IEnumerable<DocumentoIdentidad>> ListarActivosAsync();
}
