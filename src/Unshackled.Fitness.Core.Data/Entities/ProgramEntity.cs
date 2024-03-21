using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Data.Entities;

public class ProgramEntity : BaseMemberEntity
{
	public string Title { get; set; } = string.Empty;
	public ProgramTypes ProgramType { get; set; }
	public string? Description { get; set; }
	public int LengthWeeks { get; set; }
	public DateTime? DateStarted { get; set; }
	public DateTime? DateLastWorkoutUtc { get; set; }
	public int NextTemplateIndex { get; set; }

	public List<ProgramTemplateEntity> Templates { get; set; } = new();

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<ProgramEntity>, IEntityTypeConfiguration<ProgramEntity>
	{
		public override void Configure(EntityTypeBuilder<ProgramEntity> config)
		{
			base.Configure(config);

			config.ToTable("Programs");

			config.Property(x => x.Title)
				.HasMaxLength(255)
				.IsRequired();

			config.HasIndex(x => new { x.MemberId, x.Title });
		}
	}
}
