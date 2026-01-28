using PowerMas.Api.Contracts;
using PowerMas.Api.Domain;

namespace PowerMas.Api.Services;

/// <summary>
/// Servicio para Beneficiario
/// </summary>
public interface IBeneficiarioService
{
    Task<IEnumerable<BeneficiarioDetalle>> ListarAsync();
    Task<Beneficiario> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(BeneficiarioRequest request);
    Task<int> ActualizarAsync(int id, BeneficiarioRequest request);
    Task<int> EliminarAsync(int id);
}
