using Entities.enums;
using System.ComponentModel;

namespace API.Contracts.Meetings;

public class CancelMeetingRequest
{
	[Description("Область отмены повторяющихся встреч: 1 — только эта встреча, 2 — эта и следующие, 3 — вся серия.")]
	public MeetingUpdateScope Scope { get; set; } = MeetingUpdateScope.ThisMeeting;
}
