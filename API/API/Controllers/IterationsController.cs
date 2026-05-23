using API.Contracts.Iterations;
using Application.Abstractions.Iterations;
using Application.Iterations.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/iteration")]
[Authorize]
public class IterationsController(IIterationService iterationService) : ControllerBase
{
	private readonly IIterationService _iterationService = iterationService;

	/// <summary>
	/// Создание итерации для проекта.
	/// Используется на странице команды и странице оценок по итерациям для кнопки "Добавить итерацию".
	/// </summary>
	[EndpointSummary("Добавление итерации")]
	[HttpPost]
	public async Task<ActionResult<IterationResponse>> Create(
		[FromBody] CreateIterationRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var result = await _iterationService.CreateAsync(new CreateIterationCommand
		{
			ProjectId = request.ProjectId,
			Name = request.Name,
			StartOn = request.StartOn,
			EndOn = request.EndOn,
			EvaluationCriteria = request.EvaluationCriteria
		}, cancellationToken);

		if (result is null)
		{
			return BadRequest(new { message = "Project was not found, iteration with this name already exists, or date range is invalid." });
		}

		return Ok(MapIteration(result));
	}

	/// <summary>
	/// Изменение итерации и текста критериев оценивания.
	/// Используется для кнопки "Изменить итерацию" и редактирования блока "Критерии оценивания".
	/// </summary>
	[EndpointSummary("Изменение итерации")]
	[HttpPut("{iterationId:guid}")]
	public async Task<ActionResult<IterationResponse>> Update(
		Guid iterationId,
		[FromBody] UpdateIterationRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var result = await _iterationService.UpdateAsync(new UpdateIterationCommand
		{
			IterationId = iterationId,
			Name = request.Name,
			StartOn = request.StartOn,
			EndOn = request.EndOn,
			EvaluationCriteria = request.EvaluationCriteria
		}, cancellationToken);

		if (result is null)
		{
			return NotFound(new { message = "Iteration was not found, duplicate name was found, or date range is invalid." });
		}

		return Ok(MapIteration(result));
	}

	/// <summary>
	/// Список итераций проекта для выпадающего списка "Выберите итерацию".
	/// </summary>
	[EndpointSummary("Список итераций проекта")]
	[HttpGet("project/{projectId:guid}")]
	public async Task<ActionResult<GetIterationsResponse>> GetByProject(Guid projectId, CancellationToken cancellationToken)
	{
		var result = await _iterationService.GetByProjectAsync(projectId, cancellationToken);
		return Ok(new GetIterationsResponse
		{
			Items = result.Select(MapIteration).ToList()
		});
	}

	/// <summary>
	/// Данные для страницы "Оценка по итерациям": проект, выбранная команда, выбранная итерация,
	/// критерии оценивания, участники команды и уже сохраненные оценки/комментарии.
	/// </summary>
	[EndpointSummary("Страница оценки по итерациям")]
	[HttpPost("score-page")]
	public async Task<ActionResult<IterationScorePageResponse>> GetScorePage(
		[FromBody] GetIterationScorePageRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var result = await _iterationService.GetScorePageAsync(new GetIterationScorePageQuery
		{
			ProjectId = request.ProjectId,
			TeamId = request.TeamId,
			IterationId = request.IterationId
		}, cancellationToken);

		if (result is null)
		{
			return NotFound(new { message = "Project was not found." });
		}

		return Ok(new IterationScorePageResponse
		{
			ProjectId = result.ProjectId,
			ProjectTitle = result.ProjectTitle,
			ProjectStatus = result.ProjectStatus,
			ProjectEvaluationCriteria = result.ProjectEvaluationCriteria,
			SelectedTeamId = result.SelectedTeamId,
			SelectedTeamName = result.SelectedTeamName,
			SelectedIterationId = result.SelectedIterationId,
			SelectedIterationName = result.SelectedIterationName,
			SelectedIterationEvaluationCriteria = result.SelectedIterationEvaluationCriteria,
			Iterations = result.Iterations.Select(MapIteration).ToList(),
			Members = result.Members.Select(member => new IterationScoreMemberResponse
			{
				MemberId = member.MemberId,
				StudentProfileId = member.StudentProfileId,
				FullName = member.FullName,
				RoleInTeam = member.RoleInTeam,
				Score = member.Score,
				Comment = member.Comment
			}).ToList()
		});
	}

	/// <summary>
	/// Сохранение оценок и комментариев участникам команды за итерацию.
	/// Если оценка участнику уже была сохранена, она обновляется тем же запросом.
	/// </summary>
	[EndpointSummary("Сохранение оценок участников за итерацию")]
	[HttpPut("{iterationId:guid}/member-scores")]
	public async Task<IActionResult> SaveMemberScores(
		Guid iterationId,
		[FromBody] SaveIterationMemberScoresRequest request,
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

		var isSuccess = await _iterationService.SaveMemberScoresAsync(new SaveIterationMemberScoresCommand
		{
			IterationId = iterationId,
			TeamId = request.TeamId,
			ScoredByUserId = userId,
			Scores = request.Scores.Select(score => new SaveIterationMemberScoreItemCommand
			{
				MemberId = score.MemberId,
				Score = score.Score,
				Comment = score.Comment
			}).ToList()
		}, cancellationToken);

		if (!isSuccess)
		{
			return BadRequest(new { message = "Iteration/team data is invalid, member does not belong to team, or score is outside 0..100." });
		}

		return NoContent();
	}

	private bool TryGetCurrentUserId(out Guid userId)
	{
		var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
		return Guid.TryParse(userIdValue, out userId);
	}

	private static IterationResponse MapIteration(IterationResult iteration)
	{
		return new IterationResponse
		{
			Id = iteration.Id,
			ProjectId = iteration.ProjectId,
			Name = iteration.Name,
			StartOn = iteration.StartOn,
			EndOn = iteration.EndOn,
			EvaluationCriteria = iteration.EvaluationCriteria,
			CreatedAt = iteration.CreatedAt,
			UpdatedAt = iteration.UpdatedAt
		};
	}
}
