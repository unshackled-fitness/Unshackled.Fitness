using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core;

public class AppState
{
	public Themes Theme { get; set; }
	public Member ActiveMember { get; private set; } = new();
	public bool IsMemberLoaded { get; private set; } = false;
	public bool IsServerError { get; private set; } = false;

	public event Action? OnActiveMemberChange;
	public event Action? OnMemberLoadedChange;
	public event Action? OnThemeChange;
	public event Action? OnServerErrorChange;

	public void SetActiveMember(Member member)
	{
		if (member.Settings.AppTheme != Theme)
		{
			Theme = member.Settings.AppTheme;
			OnThemeChange?.Invoke();
		}

		ActiveMember = member;
		SetMemberLoaded(true);
		OnActiveMemberChange?.Invoke();
	}

	public void SetMemberLoaded(bool loaded)
	{
		if (IsMemberLoaded != loaded)
		{
			IsMemberLoaded = loaded;
			OnMemberLoadedChange?.Invoke();
		}
	}

	public void SetServerError()
	{
		IsServerError = true;
		OnServerErrorChange?.Invoke();
	}
}