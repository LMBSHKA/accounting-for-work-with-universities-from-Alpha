namespace API.Contracts.Meetings;

public class CalendarDayResponse
{
	public DateOnly Date { get; set; }
	public List<MeetingResponse> Meetings { get; set; } = [];
}
