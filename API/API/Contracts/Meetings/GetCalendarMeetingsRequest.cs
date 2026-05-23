using System.ComponentModel;

namespace API.Contracts.Meetings;

public class GetCalendarMeetingsRequest
{
	[Description("Начало периода календаря.")]
	public DateTime From { get; set; }

	[Description("Конец периода календаря.")]
	public DateTime To { get; set; }

	[Description("Необязательный фильтр по проекту.")]
	public Guid? ProjectId { get; set; }

	[Description("Необязательный фильтр по команде.")]
	public Guid? TeamId { get; set; }

	[Description("Если true, в ответ попадут отмененные встречи.")]
	public bool IncludeCancelled { get; set; }
}
