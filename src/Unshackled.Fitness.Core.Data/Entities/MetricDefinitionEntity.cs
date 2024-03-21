using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Data.Entities;

public class MetricDefinitionEntity : BaseMemberEntity
{
	public string Title { get; set; } = string.Empty;
	public string? SubTitle { get; set; }
	public MetricTypes MetricType { get; set; }
	public long ListGroupId { get; set; }
	public virtual MetricDefinitionGroupEntity ListGroup { get; set; } = null!;
	public int SortOrder { get; set; }
	public string HighlightColor { get; set; } = string.Empty;
	public decimal MaxValue { get; set; }
	public bool IsArchived { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<MetricDefinitionEntity>, IEntityTypeConfiguration<MetricDefinitionEntity>
	{
		public override void Configure(EntityTypeBuilder<MetricDefinitionEntity> config)
		{
			base.Configure(config);

			config.ToTable("MetricDefinitions");

			config.HasOne(x => x.ListGroup)
				.WithMany(x => x.Definitions)
				.HasForeignKey(x => x.ListGroupId);

			config.Property(x => x.HighlightColor)
				.HasMaxLength(10)
				.IsRequired();

			config.Property(x => x.Title)
				.HasMaxLength(50)
				.IsRequired();

			config.Property(x => x.SubTitle)
				.HasMaxLength(50);

			config.Property(x => x.MaxValue)
				.HasPrecision(2, 0);

			config.HasIndex(x => new { x.MemberId, x.ListGroupId, x.SortOrder });
		}
	}
}