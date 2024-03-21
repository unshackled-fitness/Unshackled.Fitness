namespace Unshackled.Fitness.Core.Data.Entities;

public interface IDatedEntity
{
	DateTime DateCreatedUtc { get; set; }
	DateTime? DateLastModifiedUtc { get; set; }
}
