using Application.Meetings.Models;

namespace Application.Abstractions.Meetings;

public interface IMeetingService
{
	Task<CreateMeetingsResult?> CreateAsync(CreateMeetingCommand command, CancellationToken cancellationToken = default);
	Task<MeetingResult?> GetAsync(Guid meetingId, CancellationToken cancellationToken = default);
	Task<GetCalendarMeetingsResult> GetCalendarAsync(GetCalendarMeetingsQuery query, CancellationToken cancellationToken = default);
	Task<MeetingResult?> UpdateAsync(UpdateMeetingCommand command, CancellationToken cancellationToken = default);
	Task<bool> CancelAsync(CancelMeetingCommand command, CancellationToken cancellationToken = default);
}
