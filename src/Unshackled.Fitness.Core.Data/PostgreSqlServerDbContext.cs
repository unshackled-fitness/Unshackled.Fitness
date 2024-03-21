using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Configuration;

namespace Unshackled.Fitness.Core.Data;

public class PostgreSqlServerDbContext : BaseDbContext
{
	public PostgreSqlServerDbContext(DbContextOptions<PostgreSqlServerDbContext> options,
		ConnectionStrings connectionStrings,
		DbConfiguration dbConfig) : base(options, connectionStrings, dbConfig) { }

	protected override void OnConfiguring(DbContextOptionsBuilder options)
	{
		if (!string.IsNullOrEmpty(ConnectionStrings.DefaultDatabase))
		{
			string prefix = DbConfig.TablePrefix.EndsWith("_") ? DbConfig.TablePrefix : $"{DbConfig.TablePrefix}_";
			// connect to PostgreSql database
			options.UseNpgsql(ConnectionStrings.DefaultDatabase, o =>
			{
				o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
				o.MigrationsHistoryTable($"{prefix}_EFMigrationsHistory");
			});
		}
	}
}