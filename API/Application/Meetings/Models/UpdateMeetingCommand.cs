using Entities.enums;

namespace Application.Meetings.Models;

public class UpdateMeetingCommand
{
	public Guid MeetingId { get; set; }
	public Guid? TeamId { get; set; }
	public string? Title { get; set; }
	public string? Description { get; set; }
	public string? Location { get; set; }
	public DateTime? StartAt { get; set; }
	public DateTime? EndAt { get; set; }
	public string? ConnectionLink { get; set; }
	public MeetingUpdateScope Scope { get; set; } = MeetingUpdateScope.ThisMeeting;
}
