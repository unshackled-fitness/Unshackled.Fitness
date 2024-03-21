namespace Unshackled.Fitness.Core.Configuration;

public class HashIdConfiguration
{
	public string Alphabet { get; set; } = string.Empty;
	public string Salt { get; set; } = string.Empty;
	public int MinLength { get; set; }
}
