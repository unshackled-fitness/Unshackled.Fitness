using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Data.Entities;

namespace Unshackled.Fitness.Core.Data;

public class BaseDbContext : IdentityDbContext<UserEntity>
{
	protected readonly ConnectionStrings ConnectionStrings;
	protected readonly DbConfiguration DbConfig;

	public BaseDbContext(DbContextOptions<BaseDbContext> options,
		ConnectionStrings connectionStrings,
		DbConfiguration dbConfig) : base(options)
	{
		this.ConnectionStrings = connectionStrings;
		this.DbConfig = dbConfig;
	}

	protected BaseDbContext(DbContextOptions options,
		ConnectionStrings connectionStrings,
		DbConfiguration dbConfig) : base(options)
	{
		this.ConnectionStrings = connectionStrings;
		this.DbConfig = dbConfig;
	}

	public DbSet<ExerciseEntity> Exercises => Set<ExerciseEntity>();
	public DbSet<ExportFileEntity> ExportFiles => Set<ExportFileEntity>();
	public DbSet<MemberMetaEntity> MemberMeta => Set<MemberMetaEntity>();
	public DbSet<MemberEntity> Members => Set<MemberEntity>();
	public DbSet<MetricDefinitionGroupEntity> MetricDefinitionGroups => Set<MetricDefinitionGroupEntity>();
	public DbSet<MetricDefinitionEntity> MetricDefinitions => Set<MetricDefinitionEntity>();
	public DbSet<MetricPresetEntity> MetricPresets => Set<MetricPresetEntity>();
	public DbSet<MetricEntity> Metrics => Set<MetricEntity>();
	public DbSet<ProgramEntity> Programs => Set<ProgramEntity>();
	public DbSet<ProgramTemplateEntity> ProgramTemplates => Set<ProgramTemplateEntity>();
	public DbSet<WorkoutEntity> Workouts => Set<WorkoutEntity>();
	public DbSet<WorkoutSetEntity> WorkoutSets => Set<WorkoutSetEntity>();
	public DbSet<WorkoutSetGroupEntity> WorkoutSetGroups => Set<WorkoutSetGroupEntity>();
	public DbSet<WorkoutTaskEntity> WorkoutTasks => Set<WorkoutTaskEntity>();
	public DbSet<WorkoutTemplateEntity> WorkoutTemplates => Set<WorkoutTemplateEntity>();
	public DbSet<WorkoutTemplateSetEntity> WorkoutTemplateSets => Set<WorkoutTemplateSetEntity>();
	public DbSet<WorkoutTemplateSetGroupEntity> WorkoutTemplateSetGroups => Set<WorkoutTemplateSetGroupEntity>();
	public DbSet<WorkoutTemplateTaskEntity> WorkoutTemplateTasks => Set<WorkoutTemplateTaskEntity>();

	private static void ApplyDatedDefaults(EntityEntry<IDatedEntity> entry)
	{
		switch (entry.State)
		{
			case EntityState.Added:
				entry.Entity.DateCreatedUtc = DateTime.UtcNow;
				break;
			case EntityState.Modified:
				entry.Entity.DateLastModifiedUtc = DateTime.UtcNow;
				break;
		}
	}

	public override int SaveChanges()
	{
		foreach (var entry in ChangeTracker.Entries<IDatedEntity>())
		{
			ApplyDatedDefaults(entry);
		}

		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		foreach (var entry in ChangeTracker.Entries<IDatedEntity>())
		{
			ApplyDatedDefaults(entry);
		}

		return base.SaveChangesAsync(cancellationToken);
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyConfiguration(new ExerciseEntity.TypeConfiguration());
		builder.ApplyConfiguration(new ExportFileEntity.TypeConfiguration());
		builder.ApplyConfiguration(new MemberMetaEntity.TypeConfiguration());
		builder.ApplyConfiguration(new MemberEntity.TypeConfiguration());
		builder.ApplyConfiguration(new MetricDefinitionEntity.TypeConfiguration());
		builder.ApplyConfiguration(new MetricDefinitionGroupEntity.TypeConfiguration());
		builder.ApplyConfiguration(new MetricEntity.TypeConfiguration());
		builder.ApplyConfiguration(new MetricPresetEntity.TypeConfiguration());
		builder.ApplyConfiguration(new ProgramEntity.TypeConfiguration());
		builder.ApplyConfiguration(new ProgramTemplateEntity.TypeConfiguration());
		builder.ApplyConfiguration(new WorkoutEntity.TypeConfiguration());
		builder.ApplyConfiguration(new WorkoutSetEntity.TypeConfiguration());
		builder.ApplyConfiguration(new WorkoutSetGroupEntity.TypeConfiguration());
		builder.ApplyConfiguration(new WorkoutTaskEntity.TypeConfiguration());
		builder.ApplyConfiguration(new WorkoutTemplateEntity.TypeConfiguration());
		builder.ApplyConfiguration(new WorkoutTemplateSetEntity.TypeConfiguration());
		builder.ApplyConfiguration(new WorkoutTemplateSetGroupEntity.TypeConfiguration());
		builder.ApplyConfiguration(new WorkoutTemplateTaskEntity.TypeConfiguration());

		builder.Entity<UserEntity>(x =>
		{
			x.ToTable("Users");
		});

		builder.Entity<IdentityUserClaim<string>>(x =>
		{
			x.ToTable("UserClaims");
		});

		builder.Entity<IdentityUserLogin<string>>(x =>
		{
			x.ToTable("UserLogins");
		});

		builder.Entity<IdentityUserToken<string>>(x =>
		{
			x.ToTable("UserTokens");
		});

		builder.Entity<IdentityRole>(x =>
		{
			x.ToTable("Roles");
		});

		builder.Entity<IdentityRoleClaim<string>>(x =>
		{
			x.ToTable("RoleClaims");
		});

		builder.Entity<IdentityUserRole<string>>(x =>
		{
			x.ToTable("UserRoles");
		});


		foreach (var entity in builder.Model.GetEntityTypes())
		{
			entity.SetTableName(DbConfig.TablePrefix + entity.GetTableName());
		}

		foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
		{
			relationship.DeleteBehavior = DeleteBehavior.NoAction;
		}
	}
}
