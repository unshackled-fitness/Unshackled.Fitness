namespace Unshackled.Fitness.Core;

public interface IRenderStateService
{
	bool IsInteractive { get; }
	bool IsPreRender { get; }
}