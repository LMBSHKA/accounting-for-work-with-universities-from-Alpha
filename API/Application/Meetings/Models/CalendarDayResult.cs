namespace Application.Meetings.Models;

public class CalendarDayResult
{
	public DateOnly Date { get; set; }
	public List<MeetingResult> Meetings { get; set; } = [];
}
