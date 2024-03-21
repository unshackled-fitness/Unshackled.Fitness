using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Data.Entities;

public class WorkoutTaskEntity : BaseMemberEntity
{
	public long WorkoutId { get; set; }
	public virtual WorkoutEntity Workout { get; set; } = null!;
	public WorkoutTaskTypes Type { get; set; } = WorkoutTaskTypes.PreWorkout;
	public string Text { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public bool Completed { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<WorkoutTaskEntity>, IEntityTypeConfiguration<WorkoutTaskEntity>
	{
		public override void Configure(EntityTypeBuilder<WorkoutTaskEntity> config)
		{
			base.Configure(config);

			config.ToTable("WorkoutTasks");

			config.HasOne(x => x.Workout)
				.WithMany(x => x.Tasks)
				.HasForeignKey(x => x.WorkoutId);

			config.HasIndex(x => new { x.WorkoutId, x.Type, x.SortOrder });
		}
	}
}
