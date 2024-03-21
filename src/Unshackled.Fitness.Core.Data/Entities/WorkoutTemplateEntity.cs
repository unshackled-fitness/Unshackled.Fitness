using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class WorkoutTemplateEntity : BaseMemberEntity
{
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public int ExerciseCount { get; set; }
	public string? MusclesTargeted { get; set; }
	public int SetCount { get; set; }


	public List<WorkoutTemplateSetGroupEntity> Groups { get; set; } = new();
	public List<WorkoutTemplateSetEntity> Sets { get; set; } = new();
	public List<WorkoutTemplateTaskEntity> Tasks { get; set; } = new();

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<WorkoutTemplateEntity>, IEntityTypeConfiguration<WorkoutTemplateEntity>
	{
		public override void Configure(EntityTypeBuilder<WorkoutTemplateEntity> config)
		{
			base.Configure(config);

			config.ToTable("WorkoutTemplates");

			config.Property(x => x.Title)
				.HasMaxLength(255)
				.IsRequired();

			config.HasIndex(x => new { x.MemberId, x.Title });
		}
	}
}
