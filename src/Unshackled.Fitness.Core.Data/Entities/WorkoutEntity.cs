using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class WorkoutEntity : BaseMemberEntity
{
	public string Title { get; set; } = string.Empty;
	public DateTime? DateStartedUtc { get; set; }
	public DateTime? DateCompletedUtc { get; set; }
	public string? MusclesTargeted { get; set; }
	public int ExerciseCount { get; set; }
	public int SetCount { get; set; }
	public int RepCount { get; set; }
	public decimal VolumeKg { get; set; }
	public decimal VolumeLb { get; set; }
	public long? WorkoutTemplateId { get; set; }
	public int RecordSecondsCount { get; set; }
	public int RecordSecondsAtWeightCount { get; set; }
	public int RecordTargetVolumeCount { get; set; }
	public int RecordTargetWeightCount { get; set; }
	public int RecordVolumeCount { get; set; }
	public int RecordWeightCount { get; set; }

	public List<WorkoutSetGroupEntity> Groups { get; set; } = new();
	public List<WorkoutSetEntity> Sets { get; set; } = new();
	public List<WorkoutTaskEntity> Tasks { get; set; } = new();

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<WorkoutEntity>, IEntityTypeConfiguration<WorkoutEntity>
	{
		public override void Configure(EntityTypeBuilder<WorkoutEntity> config)
		{
			base.Configure(config);

			config.ToTable("Workouts");

			config.Property(x => x.Title)
				.HasMaxLength(255)
				.IsRequired();

			config.Property(x => x.VolumeLb)
				.HasPrecision(12, 3);

			config.Property(x => x.VolumeKg)
				.HasPrecision(12, 3);

			config.HasIndex(x => new { x.MemberId, x.DateStartedUtc });
		}
	}
}
