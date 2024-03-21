using FluentValidation;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Models;

public class FormMetricDefinitionModel : BaseMemberObject, IGroupedSortable, ICloneable
{
	public string Title { get; set; } = string.Empty;
	public string? SubTitle { get; set; }
	public MetricTypes MetricType { get; set; }
	public string ListGroupSid { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public string HighlightColor { get; set; } = string.Empty;
	public decimal MaxValue { get; set; }
	public bool IsArchived { get; set; }

	public object Clone()
	{
		return new FormMetricDefinitionModel
		{
			DateCreatedUtc = DateCreatedUtc,
			DateLastModifiedUtc = DateLastModifiedUtc,
			ListGroupSid = ListGroupSid,
			HighlightColor = HighlightColor,
			IsArchived = IsArchived,
			MaxValue = MaxValue,
			MemberSid = MemberSid,
			MetricType = MetricType,
			Sid = Sid,
			SortOrder = SortOrder,
			SubTitle = SubTitle,
			Title = Title
		};
	}

	public class Validator : BaseValidator<FormMetricDefinitionModel>
	{
		public Validator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required.")
				.MaximumLength(50).WithMessage("Title must not exceed 50 characters.");
		}
	}
}
