namespace Application.Meetings.Models;

public class GetCalendarMeetingsQuery
{
	public DateTime From { get; set; }
	public DateTime To { get; set; }
	public Guid? ProjectId { get; set; }
	public Guid? TeamId { get; set; }
	public bool IncludeCancelled { get; set; }
}
