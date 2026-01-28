using PowerMas.Api.Contracts;
using PowerMas.Api.Data;
using PowerMas.Api.Domain;

namespace PowerMas.Api.Services;

/// <summary>
/// Implementación del servicio Beneficiario
/// </summary>
public class BeneficiarioService : IBeneficiarioService
{
    private readonly IBeneficiarioRepository _repository;

    public BeneficiarioService(IBeneficiarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BeneficiarioDetalle>> ListarAsync()
    {
        return await _repository.ListarAsync();
    }

    public async Task<Beneficiario> ObtenerPorIdAsync(int id)
    {
        return await _repository.ObtenerPorIdAsync(id);
    }

    public async Task<int> CrearAsync(BeneficiarioRequest request)
    {
        var beneficiario = new Beneficiario
        {
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            DocumentoIdentidadId = request.DocumentoIdentidadId,
            NumeroDocumento = request.NumeroDocumento,
            FechaNacimiento = request.FechaNacimiento,
            Sexo = request.Sexo[0]
        };
        return await _repository.CrearAsync(beneficiario);
    }

    public async Task<int> ActualizarAsync(int id, BeneficiarioRequest request)
    {
        var beneficiario = new Beneficiario
        {
            Id = id,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            DocumentoIdentidadId = request.DocumentoIdentidadId,
            NumeroDocumento = request.NumeroDocumento,
            FechaNacimiento = request.FechaNacimiento,
            Sexo = request.Sexo[0]
        };
        return await _repository.ActualizarAsync(beneficiario);
    }

    public async Task<int> EliminarAsync(int id)
    {
        return await _repository.EliminarAsync(id);
    }
}
