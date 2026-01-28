using Microsoft.AspNetCore.Mvc;
using PowerMas.Api.Domain;
using PowerMas.Api.Services;

namespace PowerMas.Api.Controllers;

[ApiController]
[Route("api/documentos-identidad")]
public class DocumentosIdentidadController : ControllerBase
{
    private readonly IDocumentoIdentidadService _service;

    public DocumentosIdentidadController(IDocumentoIdentidadService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene la lista de documentos de identidad activos (para dropdown)
    /// </summary>
    [HttpGet("activos")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoIdentidad>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DocumentoIdentidad>>> GetActivos()
    {
        var documentos = await _service.ListarActivosAsync();
        return Ok(documentos);
    }
}
