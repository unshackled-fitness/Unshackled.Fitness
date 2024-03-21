using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class MetricPresetEntity : BaseMemberEntity
{
	public string Title { get; set; } = string.Empty;
	public string Settings { get; set; } = string.Empty;

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<MetricPresetEntity>, IEntityTypeConfiguration<MetricPresetEntity>
	{
		public override void Configure(EntityTypeBuilder<MetricPresetEntity> config)
		{
			base.Configure(config);

			config.ToTable("MetricPresets");

			config.Property(x => x.Title)
				.HasMaxLength(255)
				.IsRequired();

			config.HasIndex(x => new { x.MemberId, x.Title })
				.IsUnique();
		}
	}
}

