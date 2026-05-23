namespace Application.Meetings.Models;

public class GetCalendarMeetingsResult
{
	public IReadOnlyCollection<CalendarDayResult> Days { get; set; } = [];
	public int TotalCount { get; set; }
}
