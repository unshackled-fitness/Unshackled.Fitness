using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public class MemberMetaEntity
{
	public long Id { get; set; }
	public long MemberId { get; set; }
	public virtual MemberEntity Member { get; set; } = null!;
	public string MetaKey { get; set; } = string.Empty;
	public string MetaValue { get; set; } = string.Empty;

	public class TypeConfiguration : IEntityTypeConfiguration<MemberMetaEntity>
	{
		public void Configure(EntityTypeBuilder<MemberMetaEntity> config)
		{
			config.ToTable("MemberMeta");

			config.HasKey(x => x.Id);
			
			config.Property(x => x.MemberId)
				.HasMaxLength(450)
				.IsRequired();

			config.Property(x => x.MetaKey)
				.HasMaxLength(255)
				.IsRequired();

			config.HasOne(x => x.Member)
				.WithMany()
				.HasForeignKey(x => x.MemberId);

			config.HasIndex(x => new { x.MemberId, x.MetaKey })
				.IsUnique();
		}
	}
}

