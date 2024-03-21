using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class WorkoutTemplateSetGroupEntity : BaseMemberEntity
{
	public long WorkoutTemplateId { get; set; }
	public virtual WorkoutTemplateEntity WorkoutTemplate { get; set; } = null!;
	public int SortOrder { get; set; }
	public string Title { get; set; } = string.Empty;

	public virtual List<WorkoutTemplateSetEntity> Sets { get; set; } = new();

	public class TypeConfiguration : BaseMemberEntityTypeConfiguration<WorkoutTemplateSetGroupEntity>, IEntityTypeConfiguration<WorkoutTemplateSetGroupEntity>
	{
		public override void Configure(EntityTypeBuilder<WorkoutTemplateSetGroupEntity> config)
		{
			base.Configure(config);

			config.ToTable("WorkoutTemplateSetGroups");

			config.HasOne(x => x.WorkoutTemplate)
				.WithMany(x => x.Groups)
				.HasForeignKey(x => x.WorkoutTemplateId);

			config.HasIndex(x => new { x.WorkoutTemplateId, x.SortOrder });
		}
	}
}
