using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class ProgramTemplateEntity : BaseMemberEntity
{
	public long ProgramId { get; set; }
	public virtual ProgramEntity Program { get; set; } = null!;
	public long WorkoutTemplateId { get; set; }
	public virtual WorkoutTemplateEntity WorkoutTemplate { get; set; } = null!;
	public int WeekNumber { get; set; }
	public int DayNumber { get; set; }
	public int SortOrder { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<ProgramTemplateEntity>, IEntityTypeConfiguration<ProgramTemplateEntity>
	{
		public override void Configure(EntityTypeBuilder<ProgramTemplateEntity> config)
		{
			base.Configure(config);

			config.ToTable("ProgramTemplates");

			config.HasOne(x => x.Program)
				.WithMany(x => x.Templates)
				.HasForeignKey(x => x.ProgramId);

			config.HasOne(x => x.WorkoutTemplate)
				.WithMany()
				.HasForeignKey(x => x.WorkoutTemplateId);

			config.HasIndex(x => new { x.ProgramId, x.WeekNumber, x.DayNumber });
		}
	}
}
