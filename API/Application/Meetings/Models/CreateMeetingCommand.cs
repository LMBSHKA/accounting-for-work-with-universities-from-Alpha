using Entities.enums;

namespace Application.Meetings.Models;

public class CreateMeetingCommand
{
	public Guid TeamId { get; set; }
	public string? Title { get; set; }
	public string? Description { get; set; }
	public string? Location { get; set; }
	public DateTime StartAt { get; set; }
	public DateTime EndAt { get; set; }
	public string? ConnectionLink { get; set; }
	public MeetingRepeatType RepeatType { get; set; } = MeetingRepeatType.None;
	public List<DayOfWeek> DaysOfWeek { get; set; } = [];
	public DateOnly? EndOn { get; set; }
	public int? OccurrencesCount { get; set; }
	public Guid CreatedByUserId { get; set; }
}
