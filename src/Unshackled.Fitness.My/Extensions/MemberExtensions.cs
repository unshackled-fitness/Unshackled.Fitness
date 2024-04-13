using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Extensions;

public static class MemberExtensions
{
	public static async Task<MemberEntity?> AddMember(this BaseDbContext db, string email, SiteConfiguration siteConfig)
	{
		email = email.Trim().ToLower();

		if (string.IsNullOrEmpty(email))
			throw new NullReferenceException("Email cannot be empty.");

		var member = await db.Members
			.Where(m => m.Email == email)
			.SingleOrDefaultAsync();

		if (member != null)
			return member;

		using var transaction = await db.Database.BeginTransactionAsync();

		try
		{
			// Create member
			member = new MemberEntity
			{
				Email = email,
				IsActive = true,
			};
			db.Members.Add(member);
			await db.SaveChangesAsync();

			await transaction.CommitAsync();
			return member;
		}
		catch
		{
			await transaction.RollbackAsync();
			return null;
		}
	}

	public static async Task<Member> GetMember(this BaseDbContext db, MemberEntity memberEntity)
	{
		var member = new Member
		{
			DateCreatedUtc = memberEntity.DateCreatedUtc,
			DateLastModifiedUtc = memberEntity.DateLastModifiedUtc,
			Email = memberEntity.Email,
			EmailHash = memberEntity.Email.Trim().ToLower().ComputeSha256Hash(),
			Sid = memberEntity.Id.Encode(),
			IsActive = memberEntity.IsActive,
		};

		member.Settings = await db.GetMemberSettings(memberEntity.Id);

		return member;
	}

	public static async Task<bool> GetBoolMeta(this DbSet<MemberMetaEntity> memberMeta, long memberId, string key)
	{
		string? setting = await memberMeta.GetMeta(memberId, key);
		if (!string.IsNullOrEmpty(setting))
		{
			if (bool.TryParse(setting, out bool value))
			{
				return value;
			}
		}
		return false;
	}

	public static async Task<T?> GetEnumMeta<T>(this DbSet<MemberMetaEntity> memberMeta, long memberId, string key) where T : struct, Enum
	{
		string? setting = await memberMeta.GetMeta(memberId, key);
		if (!string.IsNullOrEmpty(setting))
		{
			if (Enum.TryParse(setting, out T value))
			{
				return value;
			}
		}
		return default;
	}

	public static async Task<int> GetIntMeta(this DbSet<MemberMetaEntity> memberMeta, long memberId, string key)
	{
		string? setting = await memberMeta.GetMeta(memberId, key);
		if (!string.IsNullOrEmpty(setting))
		{
			if (int.TryParse(setting, out int value))
			{
				return value;
			}
		}
		return 0;
	}

	public static async Task<long> GetLongMeta(this DbSet<MemberMetaEntity> memberMeta, long memberId, string key)
	{
		string? setting = await memberMeta.GetMeta(memberId, key);
		if (!string.IsNullOrEmpty(setting))
		{
			if (long.TryParse(setting, out long value))
			{
				return value;
			}
		}
		return 0L;
	}

	public static async Task<AppSettings> GetMemberSettings(this BaseDbContext db, long memberId)
	{
		string? settingsJson = await db.MemberMeta.GetMeta(memberId, ServerGlobals.MetaKeys.AppSettings);

		if (!string.IsNullOrEmpty(settingsJson))
		{
			return JsonSerializer.Deserialize<AppSettings>(settingsJson) ?? new();
		}

		return new();
	}

	public static async Task<string?> GetMeta(this DbSet<MemberMetaEntity> memberMeta, long memberId, string key)
	{
		return await memberMeta
			.Where(x => x.MemberId == memberId && x.MetaKey == key)
			.Select(x => x.MetaValue)
			.SingleOrDefaultAsync();
	}

	public static async Task SaveMeta(this BaseDbContext db, long memberId, string metaKey, string value)
	{
		var setting = await db.MemberMeta
			.Where(x => x.MemberId == memberId && x.MetaKey == metaKey)
			.SingleOrDefaultAsync();

		if (setting != null)
		{
			setting.MetaValue = value;
		}
		else
		{
			db.MemberMeta.Add(new MemberMetaEntity
			{
				MemberId = memberId,
				MetaKey = metaKey,
				MetaValue = value
			});
		}

		await db.SaveChangesAsync();
	}

	public static async Task SaveMemberSettings(this BaseDbContext db, long memberId, AppSettings settings)
	{
		string settingsJson = JsonSerializer.Serialize(settings);
		await db.SaveMeta(memberId, ServerGlobals.MetaKeys.AppSettings, settingsJson);
	}
}
