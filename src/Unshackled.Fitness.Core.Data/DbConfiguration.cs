namespace Unshackled.Fitness.Core.Data;

public class DbConfiguration
{
	public const string MSSQL = "mssql";
	public const string MYSQL = "mysql";
	public const string POSTGRESQL = "postgresql";

	public string DatabaseType { get; set; } = MSSQL;
	public string TablePrefix { get; set; } = "uf_";
}
