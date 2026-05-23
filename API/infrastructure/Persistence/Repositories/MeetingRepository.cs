using Application.Abstractions.Persistence;
using Application.Meetings.Models;
using Entities.enums;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MeetingRepository(AppDbContext dbContext) : Repository<Meeting>(dbContext), IMeetingRepository
{
	public async Task<MeetingResult?> GetMeetingAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await BuildMeetingProjection(DbContext.Meetings.AsNoTracking())
			.FirstOrDefaultAsync(meeting => meeting.Id == id, cancellationToken);
	}

	public async Task<GetCalendarMeetingsResult> GetCalendarMeetingsAsync(GetCalendarMeetingsQuery request, CancellationToken cancellationToken = default)
	{
		var from = request.From;
		var to = request.To;

		var query = DbContext.Meetings.AsNoTracking()
			.Where(meeting => meeting.StartAt < to && meeting.EndAt > from);

		if (!request.IncludeCancelled)
		{
			query = query.Where(meeting => !meeting.IsCancelled);
		}

		if (request.TeamId.HasValue)
		{
			query = query.Where(meeting => meeting.TeamId == request.TeamId.Value);
		}

		if (request.ProjectId.HasValue)
		{
			query = query.Where(meeting => meeting.Team != null && meeting.Team.ProjectId == request.ProjectId.Value);
		}

		var meetings = await BuildMeetingProjection(query)
			.OrderBy(meeting => meeting.StartAt)
			.ThenBy(meeting => meeting.Title)
			.ToListAsync(cancellationToken);

		var days = meetings
			.GroupBy(meeting => DateOnly.FromDateTime(meeting.StartAt))
			.OrderBy(group => group.Key)
			.Select(group => new CalendarDayResult
			{
				Date = group.Key,
				Meetings = group.ToList()
			})
			.ToList();

		return new GetCalendarMeetingsResult
		{
			Days = days,
			TotalCount = meetings.Count
		};
	}

	public async Task<IReadOnlyCollection<MeetingResult>> GetMeetingsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
	{
		return await BuildMeetingProjection(DbContext.Meetings.AsNoTracking().Where(meeting => ids.Contains(meeting.Id)))
			.OrderBy(meeting => meeting.StartAt)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyCollection<Meeting>> GetMeetingsForScopeAsync(
		Meeting meeting,
		MeetingUpdateScope scope,
		CancellationToken cancellationToken = default)
	{
		if (scope == MeetingUpdateScope.ThisMeeting || meeting.RecurrenceSeriesId is null)
		{
			return [meeting];
		}

		var query = DbContext.Meetings
			.Where(item => item.RecurrenceSeriesId == meeting.RecurrenceSeriesId);

		if (scope == MeetingUpdateScope.ThisAndFollowing)
		{
			query = query.Where(item => item.StartAt >= meeting.StartAt);
		}

		return await query
			.OrderBy(item => item.StartAt)
			.ToListAsync(cancellationToken);
	}

	private static IQueryable<MeetingResult> BuildMeetingProjection(IQueryable<Meeting> query)
	{
		return query.Select(meeting => new MeetingResult
		{
			Id = meeting.Id,
			TeamId = meeting.TeamId,
			TeamName = meeting.Team == null ? string.Empty : meeting.Team.Name,
			ProjectId = meeting.Team == null ? Guid.Empty : meeting.Team.ProjectId,
			ProjectTitle = meeting.Team == null || meeting.Team.Project == null ? string.Empty : meeting.Team.Project.Title,
			RecurrenceSeriesId = meeting.RecurrenceSeriesId,
			Title = meeting.Title,
			Description = meeting.Description,
			Location = meeting.Location,
			StartAt = meeting.StartAt,
			EndAt = meeting.EndAt,
			ConnectionLink = meeting.ConnectionLink,
			IsCancelled = meeting.IsCancelled,
			CreatedAt = meeting.CreatedAt,
			UpdatedAt = meeting.UpdatedAt
		});
	}
}
