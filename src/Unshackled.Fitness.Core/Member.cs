namespace Unshackled.Fitness.Core;

public class Member
{
	public string Sid { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string EmailHash {  get; set; } = string.Empty;
	public AppSettings Settings { get; set; } = new();
	public DateTime DateCreatedUtc { get; set; }
	public DateTime? DateLastModifiedUtc { get; set; }
	public bool IsActive { get; set; }
}
