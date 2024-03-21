using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class ExportFileEntity : BaseMemberEntity
{
	public string Container { get; set; } = string.Empty;
	public string RelativePath { get; set; } = string.Empty;
	public string FileName { get; set; } = string.Empty;
	public DateTime? DateExpirationUtc { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<ExportFileEntity>, IEntityTypeConfiguration<ExportFileEntity>
	{
		public override void Configure(EntityTypeBuilder<ExportFileEntity> config)
		{
			base.Configure(config);

			config.ToTable("ExportFiles");

			config.Property(x => x.Container)
				.HasMaxLength(255)
				.IsRequired();

			config.Property(x => x.RelativePath)
				.HasMaxLength(255)
				.IsRequired();

			config.Property(x => x.FileName)
				.HasMaxLength(50)
				.IsRequired();

			config.HasIndex(x => new { x.MemberId, x.Container, x.RelativePath });
		}
	}
}
