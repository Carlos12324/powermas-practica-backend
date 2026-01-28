using PowerMas.Api.Domain;

namespace PowerMas.Api.Data;

/// <summary>
/// Repositorio para Beneficiario
/// </summary>
public interface IBeneficiarioRepository
{
    Task<IEnumerable<BeneficiarioDetalle>> ListarAsync();
    Task<Beneficiario> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(Beneficiario beneficiario);
    Task<int> ActualizarAsync(Beneficiario beneficiario);
    Task<int> EliminarAsync(int id);
}
