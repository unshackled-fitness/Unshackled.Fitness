namespace Unshackled.Fitness.Core;

public abstract class BaseObject
{
	public string Sid { get; set; } = string.Empty;
	public DateTime DateCreatedUtc { get; set; }
	public DateTime? DateLastModifiedUtc { get; set; }
}
