using System.Data;

namespace PowerMas.Api.Data;

/// <summary>
/// Factory para crear conexiones a la base de datos
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
