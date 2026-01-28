using Microsoft.AspNetCore.Mvc;
using PowerMas.Api.Contracts;
using PowerMas.Api.Domain;
using PowerMas.Api.Services;

namespace PowerMas.Api.Controllers;

[ApiController]
[Route("api/beneficiarios")]
public class BeneficiariosController : ControllerBase
{
    private readonly IBeneficiarioService _service;

    public BeneficiariosController(IBeneficiarioService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene la lista de todos los beneficiarios
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BeneficiarioDetalle>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BeneficiarioDetalle>>> GetAll()
    {
        var beneficiarios = await _service.ListarAsync();
        return Ok(beneficiarios);
    }

    /// <summary>
    /// Obtiene un beneficiario por su Id
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Beneficiario), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Beneficiario>> GetById(int id)
    {
        var beneficiario = await _service.ObtenerPorIdAsync(id);
        return Ok(beneficiario);
    }

    /// <summary>
    /// Crea un nuevo beneficiario
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IdResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IdResponse>> Create([FromBody] BeneficiarioRequest request)
    {
        var id = await _service.CrearAsync(request);
        var response = new IdResponse { Id = id };
        return CreatedAtAction(nameof(GetById), new { id }, response);
    }

    /// <summary>
    /// Actualiza un beneficiario existente
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IdResponse>> Update(int id, [FromBody] BeneficiarioRequest request)
    {
        var resultId = await _service.ActualizarAsync(id, request);
        return Ok(new IdResponse { Id = resultId });
    }

    /// <summary>
    /// Elimina un beneficiario
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(RowsAffectedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RowsAffectedResponse>> Delete(int id)
    {
        var rowsAffected = await _service.EliminarAsync(id);
        return Ok(new RowsAffectedResponse { RowsAffected = rowsAffected });
    }
}
