using Application.Abstractions.Meetings;
using Application.Abstractions.Persistence;
using Application.Meetings.Models;
using Entities.enums;
using Entities.Models;

namespace Application.Meetings.Services;

public class MeetingService(IUnitOfWork unitOfWork) : IMeetingService
{
	private const int MaxOccurrences = 200;
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task<CreateMeetingsResult?> CreateAsync(CreateMeetingCommand command, CancellationToken cancellationToken = default)
	{
		if (command.TeamId == Guid.Empty || command.CreatedByUserId == Guid.Empty)
		{
			return null;
		}

		var team = await _unitOfWork.Teams.GetByIdAsync(command.TeamId, cancellationToken);
		if (team is null)
		{
			return null;
		}

		var startAt = ToUtc(command.StartAt);
		var endAt = ToUtc(command.EndAt);
		if (endAt <= startAt)
		{
			return null;
		}

		var now = DateTime.UtcNow;
		var dates = BuildOccurrenceDates(command, startAt);
		if (dates.Count == 0)
		{
			return null;
		}

		var duration = endAt - startAt;
		var seriesId = command.RepeatType == MeetingRepeatType.None || dates.Count == 1
			? (Guid?)null
			: Guid.NewGuid();

		var meetings = dates.Select(date =>
		{
			var occurrenceStart = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.FromTimeSpan(startAt.TimeOfDay)), DateTimeKind.Utc);
			var occurrenceEnd = occurrenceStart.Add(duration);

			return new Meeting
			{
				Id = Guid.NewGuid(),
				TeamId = command.TeamId,
				RecurrenceSeriesId = seriesId,
				Title = NormalizeOptional(command.Title) ?? team.Name,
				Description = NormalizeOptional(command.Description),
				Location = NormalizeOptional(command.Location),
				StartAt = occurrenceStart,
				EndAt = occurrenceEnd,
				ConnectionLink = NormalizeOptional(command.ConnectionLink),
				CreatedByUserId = command.CreatedByUserId,
				CreatedAt = now
			};
		}).ToList();

		foreach (var meeting in meetings)
		{
			await _unitOfWork.Meetings.AddAsync(meeting, cancellationToken);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		var savedMeetings = await _unitOfWork.Meetings.GetMeetingsByIdsAsync(meetings.Select(meeting => meeting.Id).ToList(), cancellationToken);
		return new CreateMeetingsResult
		{
			Items = savedMeetings,
			CreatedCount = savedMeetings.Count
		};
	}

	public Task<MeetingResult?> GetAsync(Guid meetingId, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.Meetings.GetMeetingAsync(meetingId, cancellationToken);
	}

	public async Task<GetCalendarMeetingsResult> GetCalendarAsync(GetCalendarMeetingsQuery query, CancellationToken cancellationToken = default)
	{
		var from = ToUtc(query.From);
		var to = ToUtc(query.To);

		if (from == default)
		{
			from = DateTime.UtcNow.Date;
		}

		if (to == default || to <= from)
		{
			to = from.AddDays(7);
		}

		query.From = from;
		query.To = to;

		return await _unitOfWork.Meetings.GetCalendarMeetingsAsync(query, cancellationToken);
	}

	public async Task<MeetingResult?> UpdateAsync(UpdateMeetingCommand command, CancellationToken cancellationToken = default)
	{
		var meeting = await _unitOfWork.Meetings.GetByIdAsync(command.MeetingId, cancellationToken);
		if (meeting is null)
		{
			return null;
		}

		if (command.TeamId.HasValue)
		{
			var team = await _unitOfWork.Teams.GetByIdAsync(command.TeamId.Value, cancellationToken);
			if (team is null)
			{
				return null;
			}
		}

		var targets = await _unitOfWork.Meetings.GetMeetingsForScopeAsync(meeting, command.Scope, cancellationToken);
		var newStart = command.StartAt.HasValue ? ToUtc(command.StartAt.Value) : (DateTime?)null;
		var newEnd = command.EndAt.HasValue ? ToUtc(command.EndAt.Value) : (DateTime?)null;

		if (newStart.HasValue && newEnd.HasValue && newEnd.Value <= newStart.Value)
		{
			return null;
		}

		var now = DateTime.UtcNow;
		foreach (var target in targets)
		{
			if (command.TeamId.HasValue)
			{
				target.TeamId = command.TeamId.Value;
			}

			if (command.Title is not null)
			{
				target.Title = NormalizeRequired(command.Title);
			}

			if (command.Description is not null)
			{
				target.Description = NormalizeOptional(command.Description);
			}

			if (command.Location is not null)
			{
				target.Location = NormalizeOptional(command.Location);
			}

			if (command.ConnectionLink is not null)
			{
				target.ConnectionLink = NormalizeOptional(command.ConnectionLink);
			}

			ApplyTimeChange(target, meeting, command.Scope, newStart, newEnd);
			target.UpdatedAt = now;
			_unitOfWork.Meetings.Update(target);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return await _unitOfWork.Meetings.GetMeetingAsync(command.MeetingId, cancellationToken);
	}

	public async Task<bool> CancelAsync(CancelMeetingCommand command, CancellationToken cancellationToken = default)
	{
		var meeting = await _unitOfWork.Meetings.GetByIdAsync(command.MeetingId, cancellationToken);
		if (meeting is null)
		{
			return false;
		}

		var targets = await _unitOfWork.Meetings.GetMeetingsForScopeAsync(meeting, command.Scope, cancellationToken);
		var now = DateTime.UtcNow;
		foreach (var target in targets)
		{
			target.IsCancelled = true;
			target.CancelledAt = now;
			target.UpdatedAt = now;
			_unitOfWork.Meetings.Update(target);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return true;
	}

	private static List<DateOnly> BuildOccurrenceDates(CreateMeetingCommand command, DateTime startAt)
	{
		if (command.RepeatType == MeetingRepeatType.None)
		{
			return [DateOnly.FromDateTime(startAt)];
		}

		if (command.RepeatType != MeetingRepeatType.Weekly)
		{
			return [];
		}

		var startDate = DateOnly.FromDateTime(startAt);
		var maxByCount = command.OccurrencesCount.HasValue
			? Math.Clamp(command.OccurrencesCount.Value, 1, MaxOccurrences)
			: command.EndOn.HasValue ? MaxOccurrences : 1;
		var endDate = command.EndOn ?? startDate.AddDays(maxByCount * 7);
		var days = command.DaysOfWeek.Count == 0
			? new List<DayOfWeek> { startAt.DayOfWeek }
			: command.DaysOfWeek.Distinct().ToList();

		var result = new List<DateOnly>();
		for (var date = startDate; date <= endDate && result.Count < maxByCount; date = date.AddDays(1))
		{
			if (days.Contains(date.DayOfWeek))
			{
				result.Add(date);
			}
		}

		if (result.Count == 0)
		{
			result.Add(startDate);
		}

		return result;
	}

	private static void ApplyTimeChange(
		Meeting target,
		Meeting requestedMeeting,
		MeetingUpdateScope scope,
		DateTime? newStart,
		DateTime? newEnd)
	{
		if (!newStart.HasValue && !newEnd.HasValue)
		{
			return;
		}

		if (scope == MeetingUpdateScope.ThisMeeting || target.Id == requestedMeeting.Id && requestedMeeting.RecurrenceSeriesId is null)
		{
			if (newStart.HasValue)
			{
				target.StartAt = newStart.Value;
			}

			if (newEnd.HasValue)
			{
				target.EndAt = newEnd.Value;
			}

			return;
		}

		var targetDate = DateOnly.FromDateTime(target.StartAt);
		var startTime = newStart?.TimeOfDay ?? target.StartAt.TimeOfDay;
		var endTime = newEnd?.TimeOfDay ?? target.EndAt.TimeOfDay;
		var changedStart = DateTime.SpecifyKind(targetDate.ToDateTime(TimeOnly.FromTimeSpan(startTime)), DateTimeKind.Utc);
		var changedEnd = DateTime.SpecifyKind(targetDate.ToDateTime(TimeOnly.FromTimeSpan(endTime)), DateTimeKind.Utc);

		if (changedEnd <= changedStart)
		{
			changedEnd = changedEnd.AddDays(1);
		}

		target.StartAt = changedStart;
		target.EndAt = changedEnd;
	}

	private static DateTime ToUtc(DateTime value)
	{
		return value.Kind switch
		{
			DateTimeKind.Utc => value,
			DateTimeKind.Local => value.ToUniversalTime(),
			_ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
		};
	}

	private static string NormalizeRequired(string value)
	{
		return value.Trim();
	}

	private static string? NormalizeOptional(string? value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
	}
}
