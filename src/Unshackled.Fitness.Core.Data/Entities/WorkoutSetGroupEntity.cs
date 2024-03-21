using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class WorkoutSetGroupEntity : BaseMemberEntity
{
	public long WorkoutId { get; set; }
	public virtual WorkoutEntity Workout { get; set; } = null!;
	public int SortOrder { get; set; }
	public string Title { get; set; } = string.Empty;

	public virtual List<WorkoutSetEntity> Sets { get; set; } = new();

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<WorkoutSetGroupEntity>, IEntityTypeConfiguration<WorkoutSetGroupEntity>
	{
		public override void Configure(EntityTypeBuilder<WorkoutSetGroupEntity> config)
		{
			base.Configure(config);

			config.ToTable("WorkoutSetGroups");

			config.HasOne(x => x.Workout)
				.WithMany(x => x.Groups)
				.HasForeignKey(x => x.WorkoutId);

			config.HasIndex(x => new { x.WorkoutId, x.SortOrder });
		}
	}
}
