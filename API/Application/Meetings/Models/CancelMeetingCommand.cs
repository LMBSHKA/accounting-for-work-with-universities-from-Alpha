using Entities.enums;

namespace Application.Meetings.Models;

public class CancelMeetingCommand
{
	public Guid MeetingId { get; set; }
	public MeetingUpdateScope Scope { get; set; } = MeetingUpdateScope.ThisMeeting;
}
