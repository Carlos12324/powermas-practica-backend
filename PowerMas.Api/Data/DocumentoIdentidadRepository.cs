using System.Data;
using Dapper;
using PowerMas.Api.Domain;

namespace PowerMas.Api.Data;

/// <summary>
/// Implementación del repositorio DocumentoIdentidad
/// </summary>
public class DocumentoIdentidadRepository : IDocumentoIdentidadRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DocumentoIdentidadRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<DocumentoIdentidad>> ListarActivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<DocumentoIdentidad>(
            "dbo.sp_DocumentoIdentidad_ListarActivos",
            commandType: CommandType.StoredProcedure);
    }
}
