namespace Unshackled.Fitness.Core.Models;

public interface IRenderStateService
{
	bool IsInteractive { get; }
	bool IsPreRender { get; }
}