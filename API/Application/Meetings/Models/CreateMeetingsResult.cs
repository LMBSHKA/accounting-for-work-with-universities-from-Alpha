namespace Application.Meetings.Models;

public class CreateMeetingsResult
{
	public IReadOnlyCollection<MeetingResult> Items { get; set; } = [];
	public int CreatedCount { get; set; }
}
