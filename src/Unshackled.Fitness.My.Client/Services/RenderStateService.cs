using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Services;

public class RenderStateService : IRenderStateService
{
	public bool IsInteractive => true;
	public bool IsPreRender => false;
}
