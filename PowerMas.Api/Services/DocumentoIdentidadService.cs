using PowerMas.Api.Data;
using PowerMas.Api.Domain;

namespace PowerMas.Api.Services;

/// <summary>
/// Implementación del servicio DocumentoIdentidad
/// </summary>
public class DocumentoIdentidadService : IDocumentoIdentidadService
{
    private readonly IDocumentoIdentidadRepository _repository;

    public DocumentoIdentidadService(IDocumentoIdentidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DocumentoIdentidad>> ListarActivosAsync()
    {
        return await _repository.ListarActivosAsync();
    }
}
