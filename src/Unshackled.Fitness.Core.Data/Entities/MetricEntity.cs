using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class MetricEntity : BaseMemberEntity
{
	public long MetricDefinitionId { get; set; }
	public virtual MetricDefinitionEntity MetricDefinition { get; set; } = null!;
	public DateTime DateRecorded { get; set; }
	public decimal RecordedValue { get; set; }

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<MetricEntity>, IEntityTypeConfiguration<MetricEntity>
	{
		public override void Configure(EntityTypeBuilder<MetricEntity> config)
		{
			base.Configure(config);

			config.ToTable("Metrics");

			config.HasOne(x => x.MetricDefinition)
				.WithMany()
				.HasForeignKey(x => x.MetricDefinitionId);

			config.Property(x => x.RecordedValue)
				.HasPrecision(15, 3);

			config.HasIndex(x => new { x.MemberId, x.MetricDefinitionId, x.DateRecorded });
		}
	}
}
