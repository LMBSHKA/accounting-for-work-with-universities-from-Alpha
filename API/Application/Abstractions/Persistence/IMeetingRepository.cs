using Application.Meetings.Models;
using Entities.Models;

namespace Application.Abstractions.Persistence;

public interface IMeetingRepository : IRepository<Meeting>
{
	Task<MeetingResult?> GetMeetingAsync(Guid id, CancellationToken cancellationToken = default);
	Task<GetCalendarMeetingsResult> GetCalendarMeetingsAsync(GetCalendarMeetingsQuery query, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<MeetingResult>> GetMeetingsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);
	Task<IReadOnlyCollection<Meeting>> GetMeetingsForScopeAsync(Meeting meeting, Entities.enums.MeetingUpdateScope scope, CancellationToken cancellationToken = default);
}
