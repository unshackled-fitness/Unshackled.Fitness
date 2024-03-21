using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Data.Entities;

public class WorkoutTemplateTaskEntity : BaseMemberEntity
{
	public long WorkoutTemplateId { get; set; }
	public virtual WorkoutTemplateEntity WorkoutTemplate { get; set; } = null!;
	public WorkoutTaskTypes Type { get; set; } = WorkoutTaskTypes.PreWorkout;
	public string Text { get; set; } = string.Empty;
	public int SortOrder { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<WorkoutTemplateTaskEntity>, IEntityTypeConfiguration<WorkoutTemplateTaskEntity>
	{
		public override void Configure(EntityTypeBuilder<WorkoutTemplateTaskEntity> config)
		{
			base.Configure(config);

			config.ToTable("WorkoutTemplateTasks");

			config.HasOne(x => x.WorkoutTemplate)
				.WithMany(x => x.Tasks)
				.HasForeignKey(x => x.WorkoutTemplateId);

			config.HasIndex(x => new { x.WorkoutTemplateId, x.Type, x.SortOrder });
		}
	}
}
