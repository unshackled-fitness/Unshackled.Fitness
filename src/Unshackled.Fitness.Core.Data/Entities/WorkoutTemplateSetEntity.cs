using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Data.Entities;

public class WorkoutTemplateSetEntity : BaseMemberEntity
{
	public long WorkoutTemplateId { get; set; }
	public virtual WorkoutTemplateEntity WorkoutTemplate { get; set; } = null!;
	public long ListGroupId { get; set; }
	public virtual WorkoutTemplateSetGroupEntity ListGroup { get; set; } = null!;
	public long ExerciseId { get; set; }
	public virtual ExerciseEntity Exercise { get; set; } = null!;
	public SetMetricTypes SetMetricType { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public int SortOrder { get; set; }
	public RepModes RepMode { get; set; }
	public int RepsTarget { get; set; }
	public int SecondsTarget { get; set; }
	public int IntensityTarget { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<WorkoutTemplateSetEntity>, IEntityTypeConfiguration<WorkoutTemplateSetEntity>
	{
		public override void Configure(EntityTypeBuilder<WorkoutTemplateSetEntity> config)
		{
			base.Configure(config);

			config.ToTable("WorkoutTemplateSets");

			config.HasOne(x => x.WorkoutTemplate)
				.WithMany(x => x.Sets)
				.HasForeignKey(x => x.WorkoutTemplateId);

			config.HasOne(x => x.ListGroup)
				.WithMany(x => x.Sets)
				.HasForeignKey(x => x.ListGroupId);

			config.HasOne(x => x.Exercise)
				.WithMany()
				.HasForeignKey(x => x.ExerciseId);

			config.HasIndex(x => new { x.WorkoutTemplateId, x.SortOrder });
		}
	}
}
