namespace API.Contracts.Meetings;

public class CreateMeetingsResponse
{
	public List<MeetingResponse> Items { get; set; } = [];
	public int CreatedCount { get; set; }
}
