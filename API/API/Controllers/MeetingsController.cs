using API.Contracts.Meetings;
using Application.Abstractions.Meetings;
using Application.Meetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/meeting")]
[Authorize]
public class MeetingsController(IMeetingService meetingService) : ControllerBase
{
	private readonly IMeetingService _meetingService = meetingService;

	/// <summary>
	/// Получение встреч для страницы календаря.
	/// </summary>
	[EndpointSummary("Календарь встреч")]
	[HttpPost("calendar")]
	public async Task<ActionResult<GetCalendarMeetingsResponse>> GetCalendar(
		[FromBody] GetCalendarMeetingsRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var result = await _meetingService.GetCalendarAsync(new GetCalendarMeetingsQuery
		{
			From = request.From,
			To = request.To,
			ProjectId = request.ProjectId,
			TeamId = request.TeamId,
			IncludeCancelled = request.IncludeCancelled
		}, cancellationToken);

		return Ok(new GetCalendarMeetingsResponse
		{
			Days = result.Days.Select(day => new CalendarDayResponse
			{
				Date = day.Date,
				Meetings = day.Meetings.Select(MapMeeting).ToList()
			}).ToList(),
			TotalCount = result.TotalCount
		});
	}

	/// <summary>
	/// Получение карточки встречи по идентификатору.
	/// </summary>
	[EndpointSummary("Карточка встречи")]
	[HttpGet("{meetingId:guid}")]
	public async Task<ActionResult<MeetingResponse>> GetById(Guid meetingId, CancellationToken cancellationToken)
	{
		var meeting = await _meetingService.GetAsync(meetingId, cancellationToken);
		if (meeting is null)
		{
			return NotFound(new { message = "Meeting was not found." });
		}

		return Ok(MapMeeting(meeting));
	}

	/// <summary>
	/// Создание одной встречи или серии еженедельных встреч.
	/// RepeatType: 1 — без повторения, 2 — каждую неделю.
	/// DaysOfWeek: 1 — понедельник, 2 — вторник, 3 — среда, 4 — четверг, 5 — пятница.
	/// </summary>
	[EndpointSummary("Назначение встречи")]
	[HttpPost]
	public async Task<ActionResult<CreateMeetingsResponse>> Create(
		[FromBody] CreateMeetingRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		if (!TryGetCurrentUserId(out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		var result = await _meetingService.CreateAsync(new CreateMeetingCommand
		{
			TeamId = request.TeamId,
			Title = request.Title,
			Description = request.Description,
			Location = request.Location,
			StartAt = request.StartAt,
			EndAt = request.EndAt,
			ConnectionLink = request.ConnectionLink,
			RepeatType = request.RepeatType,
			DaysOfWeek = request.DaysOfWeek,
			EndOn = request.EndOn,
			OccurrencesCount = request.OccurrencesCount,
			CreatedByUserId = userId
		}, cancellationToken);

		if (result is null)
		{
			return BadRequest(new { message = "Meeting data is invalid, team was not found, or end time must be greater than start time." });
		}

		return Ok(new CreateMeetingsResponse
		{
			Items = result.Items.Select(MapMeeting).ToList(),
			CreatedCount = result.CreatedCount
		});
	}

	/// <summary>
	/// Изменение встречи.
	/// Scope: 1 — только эта встреча, 2 — эта и следующие встречи серии, 3 — вся серия.
	/// </summary>
	[EndpointSummary("Изменение встречи")]
	[HttpPut("{meetingId:guid}")]
	public async Task<ActionResult<MeetingResponse>> Update(
		Guid meetingId,
		[FromBody] UpdateMeetingRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var meeting = await _meetingService.UpdateAsync(new UpdateMeetingCommand
		{
			MeetingId = meetingId,
			TeamId = request.TeamId,
			Title = request.Title,
			Description = request.Description,
			Location = request.Location,
			StartAt = request.StartAt,
			EndAt = request.EndAt,
			ConnectionLink = request.ConnectionLink,
			Scope = request.Scope
		}, cancellationToken);

		if (meeting is null)
		{
			return NotFound(new { message = "Meeting or selected team was not found, or time data is invalid." });
		}

		return Ok(MapMeeting(meeting));
	}

	/// <summary>
	/// Отмена встречи.
	/// Scope: 1 — только эта встреча, 2 — эта и следующие встречи серии, 3 — вся серия.
	/// </summary>
	[EndpointSummary("Отмена встречи")]
	[HttpDelete("{meetingId:guid}")]
	public async Task<IActionResult> Cancel(
		Guid meetingId,
		[FromQuery] Entities.enums.MeetingUpdateScope scope = Entities.enums.MeetingUpdateScope.ThisMeeting,
		CancellationToken cancellationToken = default)
	{
		var isSuccess = await _meetingService.CancelAsync(new CancelMeetingCommand
		{
			MeetingId = meetingId,
			Scope = scope
		}, cancellationToken);

		if (!isSuccess)
		{
			return NotFound(new { message = "Meeting was not found." });
		}

		return NoContent();
	}

	private bool TryGetCurrentUserId(out Guid userId)
	{
		var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
		return Guid.TryParse(userIdValue, out userId);
	}

	private static MeetingResponse MapMeeting(MeetingResult meeting)
	{
		return new MeetingResponse
		{
			Id = meeting.Id,
			TeamId = meeting.TeamId,
			TeamName = meeting.TeamName,
			ProjectId = meeting.ProjectId,
			ProjectTitle = meeting.ProjectTitle,
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
		};
	}
}
