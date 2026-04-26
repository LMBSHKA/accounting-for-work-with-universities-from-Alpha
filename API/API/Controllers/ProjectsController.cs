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
            CreatedByUserId = userId
        }, cancellationToken);

        return Ok();
    }
}
