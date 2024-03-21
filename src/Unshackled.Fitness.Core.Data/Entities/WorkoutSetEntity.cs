using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Data.Entities;

public class WorkoutSetEntity : BaseMemberEntity
{
	public long WorkoutId { get; set; }
	public virtual WorkoutEntity Workout { get; set; } = null!;
	public long ExerciseId { get; set; }
	public virtual ExerciseEntity Exercise { get; set; } = null!;
	public SetMetricTypes SetMetricType { get; set; }
	public long ListGroupId { get; set; }
	public virtual WorkoutSetGroupEntity ListGroup { get; set; } = null!;
	public bool IsTrackingSplit { get; set; }
	public int IntensityTarget { get; set; }
	public RepModes RepMode { get; set; }
	public int RepsTarget { get; set; }
	public int Reps { get; set; }
	public int RepsLeft { get; set; }
	public int RepsRight { get; set; }
	public int SecondsTarget { get; set; }
	public int Seconds { get; set; }
	public int SecondsLeft { get; set; }
	public int SecondsRight { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public int SortOrder { get; set; }
	public decimal WeightLb { get; set; }
	public decimal WeightKg { get; set; }
	public decimal VolumeLb { get; set; }
	public decimal VolumeKg { get; set; }
	public DateTime? DateRecordedUtc { get; set; }
	public bool IsBestSetBySeconds { get; set; }
	public bool IsBestSetByWeight { get; set; }
	public bool IsBestSetByVolume { get; set; }
	public bool IsRecordSeconds { get; set; }
	public bool IsRecordSecondsAtWeight { get; set; }
	public bool IsRecordTargetVolume { get; set; }
	public bool IsRecordTargetWeight { get; set; }
	public bool IsRecordVolume { get; set; }
	public bool IsRecordWeight { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<WorkoutSetEntity>, IEntityTypeConfiguration<WorkoutSetEntity>
	{
		public override void Configure(EntityTypeBuilder<WorkoutSetEntity> config)
		{
			base.Configure(config);

			config.ToTable("WorkoutSets");

			config.Property(x => x.WeightLb)
				.HasPrecision(7, 3); 
			
			config.Property(x => x.WeightKg)
				.HasPrecision(7, 3);

			config.Property(x => x.VolumeLb)
				.HasPrecision(10, 3);

			config.Property(x => x.VolumeKg)
				.HasPrecision(10, 3);

			config.HasOne(x => x.Workout)
				.WithMany(x => x.Sets)
				.HasForeignKey(x => x.WorkoutId);

			config.HasOne(x => x.ListGroup)
				.WithMany(x => x.Sets)
				.HasForeignKey(x => x.ListGroupId);

			config.HasOne(x => x.Exercise)
				.WithMany()
				.HasForeignKey(x => x.ExerciseId);

			config.HasIndex(x => new { x.WorkoutId, x.SortOrder });
		}
	}
}
