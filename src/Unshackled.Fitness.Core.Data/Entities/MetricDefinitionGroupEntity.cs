using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class MetricDefinitionGroupEntity : BaseMemberEntity
{
	public string Title { get; set; } = string.Empty;
	public int SortOrder { get; set; }

	public virtual List<MetricDefinitionEntity> Definitions { get; set; } = new();

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<MetricDefinitionGroupEntity>, IEntityTypeConfiguration<MetricDefinitionGroupEntity>
	{
		public override void Configure(EntityTypeBuilder<MetricDefinitionGroupEntity> config)
		{
			base.Configure(config);

			config.ToTable("MetricDefinitionGroups");

			config.Property(x => x.Title)
				.HasMaxLength(255)
				.IsRequired();

			config.HasIndex(x => new { x.MemberId, x.SortOrder });
		}
	}
}