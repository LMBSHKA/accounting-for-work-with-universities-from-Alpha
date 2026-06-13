using API.Contracts.Projects;
using Application.Abstractions.Projects;
using Application.Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/project")]
[Authorize]
public class ProjectsController(IProjectService projectService) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;

    [EndpointSummary("Добавление проекта")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new { message = "User identifier claim is missing." });
        }

        var project = await _projectService.CreateAsync(new CreateProject
        {
            Title = request.Title,
            ShortTitle = request.ShortTitle,
            Description = request.Description,
            Goal = request.Goal,
            Status = request.Status,
            Tasks = request.Tasks,
            Mvp = request.Mvp,
            EvaluationCriteria = request.EvaluationCriteria,
            Deadline = request.Deadline,
            CreatedByUserId = userId
        }, cancellationToken);

        return Ok();
    }

	[EndpointSummary("Список проектов")]
	[HttpPost("list")]
	public async Task<ActionResult<GetProjectsResponse>> GetProjects(
		[FromBody] GetProjectsRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var result = await _projectService.GetProjectsAsync(new GetProjectsQuery
		{
			Search = request.Search,
			Statuses = request.Statuses,
			Offset = request.Offset,
			Limit = request.Limit
		}, cancellationToken);

		return Ok(new GetProjectsResponse
		{
			Items = result.Items.Select(project => new ProjectListItemResponse
			{
				Id = project.Id,
				Title = project.Title,
				Description = project.Description,
				Goal = project.Goal,
				Mvp = project.Mvp,
				EvaluationCriteria = project.EvaluationCriteria,
				Deadline = project.Deadline,
				Status = project.Status,
				TeamsCount = project.TeamsCount,
			}).ToList(),
			TotalCount = result.TotalCount,
			Offset = result.Offset,
			Limit = result.Limit,
			LoadedCount = result.LoadedCount,
			HasMore = result.HasMore,
			NextOffset = result.NextOffset
		});
	}
	/// <summary>
	/// Обновление статуса проекта.
	/// Можно перевести проект из любого статуса в любой другой статус.
	/// </summary>
	[EndpointSummary("Обновление статуса проекта")]
	[HttpPatch("{projectId:guid}/status")]
	public async Task<IActionResult> UpdateStatus(
		Guid projectId,
		[FromBody] UpdateProjectStatusRequest request,
		CancellationToken cancellationToken)
	{
		if (!Enum.IsDefined(request.Status))
		{
			ModelState.AddModelError(nameof(request.Status), "Project status is invalid.");
		}

		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (!Guid.TryParse(userIdValue, out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		var isSuccess = await _projectService.UpdateStatusAsync(new UpdateProjectStatusCommand
		{
			ProjectId = projectId,
			Status = request.Status,
			ChangedByUserId = userId
		}, cancellationToken);

		if (!isSuccess)
		{
			return NotFound(new { message = "Project was not found." });
		}

		return NoContent();
	}

	/// <summary>
	/// Завершение проекта со страницы команды.
	/// После вызова статус проекта становится 4 — Completed.
	/// </summary>
	[EndpointSummary("Завершение проекта")]
	[HttpPost("{projectId:guid}/complete")]
	public async Task<IActionResult> CompleteProject(Guid projectId, CancellationToken cancellationToken)
	{
		var userIdValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (!Guid.TryParse(userIdValue, out var userId))
		{
			return Unauthorized(new { message = "User identifier claim is missing." });
		}

		var isSuccess = await _projectService.CompleteProjectAsync(projectId, userId, cancellationToken);
		if (!isSuccess)
		{
			return NotFound(new { message = "Project was not found." });
		}

		return NoContent();
	}

	/// <summary>
	/// История изменения статусов проекта для кнопки "Посмотреть историю".
	/// </summary>
	[EndpointSummary("История статусов проекта")]
	[HttpGet("{projectId:guid}/status-history")]
	public async Task<ActionResult<List<ProjectStatusHistoryResponse>>> GetStatusHistory(Guid projectId, CancellationToken cancellationToken)
	{
		var history = await _projectService.GetStatusHistoryAsync(projectId, cancellationToken);
		if (history is null)
		{
			return NotFound(new { message = "Project was not found." });
		}

		return Ok(history.Select(item => new ProjectStatusHistoryResponse
		{
			Id = item.Id,
			ProjectId = item.ProjectId,
			Status = item.Status,
			ChangedByUserId = item.ChangedByUserId,
			ChangedByUserFullName = item.ChangedByUserFullName,
			ChangeComment = item.ChangeComment,
			ChangedAt = item.ChangedAt
		}).ToList());
	}

}
