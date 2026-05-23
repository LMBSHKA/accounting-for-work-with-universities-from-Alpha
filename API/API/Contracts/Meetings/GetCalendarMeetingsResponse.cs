namespace API.Contracts.Meetings;

public class GetCalendarMeetingsResponse
{
	public List<CalendarDayResponse> Days { get; set; } = [];
	public int TotalCount { get; set; }
}
