using System.Data;
using Dapper;
using PowerMas.Api.Domain;

namespace PowerMas.Api.Data;

/// <summary>
/// Implementación del repositorio Beneficiario
/// </summary>
public class BeneficiarioRepository : IBeneficiarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BeneficiarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<BeneficiarioDetalle>> ListarAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<BeneficiarioDetalle>(
            "dbo.sp_Beneficiario_Listar",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Beneficiario> ObtenerPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<Beneficiario>(
            "dbo.sp_Beneficiario_ObtenerPorId",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CrearAsync(Beneficiario beneficiario)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleAsync<int>(
            "dbo.sp_Beneficiario_Crear",
            new
            {
                beneficiario.Nombres,
                beneficiario.Apellidos,
                beneficiario.DocumentoIdentidadId,
                beneficiario.NumeroDocumento,
                beneficiario.FechaNacimiento,
                beneficiario.Sexo
            },
            commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<int> ActualizarAsync(Beneficiario beneficiario)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleAsync<int>(
            "dbo.sp_Beneficiario_Actualizar",
            new
            {
                beneficiario.Id,
                beneficiario.Nombres,
                beneficiario.Apellidos,
                beneficiario.DocumentoIdentidadId,
                beneficiario.NumeroDocumento,
                beneficiario.FechaNacimiento,
                beneficiario.Sexo
            },
            commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<int> EliminarAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleAsync<int>(
            "dbo.sp_Beneficiario_Eliminar",
            new { Id = id },
            commandType: CommandType.StoredProcedure);
        return result;
    }
}
