using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Data.Entities;

public class ExerciseEntity : BaseMemberEntity
{
	public Guid? MatchId { get; set; }
	public string Muscles { get; set; } = string.Empty;
	public string Equipment { get; set; } = string.Empty;
	public SetMetricTypes DefaultSetMetricType { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? Notes { get; set; }
	public WorkoutSetTypes DefaultSetType { get; set; }
	public bool IsTrackingSplit { get; set; }
	public bool IsArchived { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<ExerciseEntity>, IEntityTypeConfiguration<ExerciseEntity>
	{
		public override void Configure(EntityTypeBuilder<ExerciseEntity> config)
		{
			base.Configure(config);

			config.ToTable("Exercises");

			config.Property(x => x.MatchId)
				.ValueGeneratedNever();

			config.Property(x => x.Title)
				.HasMaxLength(255)
				.IsRequired();

			config.HasIndex(x => new { x.MemberId, x.Title, x.IsArchived });
		}
	}
}
