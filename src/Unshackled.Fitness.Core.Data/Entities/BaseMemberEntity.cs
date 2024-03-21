using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Unshackled.Fitness.Core.Data.Entities;

public abstract class BaseMemberEntity : BaseEntity
{
	public long MemberId { get; set; }
	public virtual MemberEntity Member { get; set; } = null!;
}

public abstract class BaseMemberEntityTypeConfiguration<TEntity> : BaseEntityTypeConfiguration<TEntity>, IEntityTypeConfiguration<TEntity> where TEntity : BaseMemberEntity
{
	public override void Configure(EntityTypeBuilder<TEntity> config)
	{
		base.Configure(config);

		config.HasOne(x => x.Member)
			.WithMany()
			.HasForeignKey(x => x.MemberId);

		config.HasIndex(x => x.MemberId);
	}

	public void Configure(EntityTypeBuilder<TEntity> config, Expression<Func<MemberEntity, IEnumerable<TEntity>?>> navigationExpression)
	{
		base.Configure(config);

		config.HasOne(x => x.Member)
			.WithMany(navigationExpression)
			.HasForeignKey(x => x.MemberId);

		config.HasIndex(x => x.MemberId);
	}
}
