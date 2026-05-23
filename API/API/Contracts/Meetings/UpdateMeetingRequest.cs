using Entities.enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Meetings;

public class UpdateMeetingRequest
{
	public Guid? TeamId { get; set; }

	[MaxLength(250)]
	public string? Title { get; set; }

	[MaxLength(4000)]
	public string? Description { get; set; }

	[MaxLength(500)]
	public string? Location { get; set; }

	public DateTime? StartAt { get; set; }
	public DateTime? EndAt { get; set; }

	[MaxLength(1024)]
	public string? ConnectionLink { get; set; }

	[Description("Область изменения повторяющихся встреч: 1 — только эта встреча, 2 — эта и следующие, 3 — вся серия.")]
	public MeetingUpdateScope Scope { get; set; } = MeetingUpdateScope.ThisMeeting;
}
