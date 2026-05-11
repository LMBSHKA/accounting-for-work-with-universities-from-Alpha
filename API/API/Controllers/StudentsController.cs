using API.Contracts.Students;
using Application.Abstractions.Students;
using Application.Students.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/student")]
[Authorize]
public class StudentsController(IStudentService studentService) : ControllerBase
{
	private readonly IStudentService _studentService = studentService;

	[EndpointSummary("Создание студента")]
	[HttpPost]
	public async Task<ActionResult<StudentResponse>> Create(
		[FromBody] CreateStudentRequest request,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
		{
			return ValidationProblem(ModelState);
		}

		var student = await _studentService.CreateAsync(new CreateStudentCommand
		{
			FullName = request.FullName,
			Email = request.Email,
			RoleInTeam = request.RoleInTeam
		}, cancellationToken);

		if (student is null)
		{
			return Conflict(new { message = "Student with this email already exists or request data is invalid." });
		}

		return Ok(new StudentResponse
		{
			Id = student.Id,
			FullName = student.FullName,
			Email = student.Email,
			RoleInTeam = student.RoleInTeam,
			UpdatedAt = student.UpdatedAt
		});
	}

	[EndpointSummary("Создание команды")]
	[HttpPost("team")]
	public async Task<ActionResult<TeamResponse>> CreateTeam(
		[FromBody] CreateTeamRequest request,
		CancellationToken cancellationToken)
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

		var team = await _studentService.CreateTeamAsync(new CreateTeamCommand
		{
			ProjectId = request.ProjectId,
			Name = request.Name,
			Skills = request.Skills,
			CreatedByUserId = userId,
			StudentProfileIds = request.StudentProfileIds
		}, cancellationToken);

		if (team is null)
		{
			return Conflict(new { message = "Team with this name already exists in project, project or students were not found, or request data is invalid." });
		}

		return Ok(new TeamResponse
		{
			Id = team.Id,
			ProjectId = team.ProjectId,
			Name = team.Name,
			Skills = team.Skills,
			CreatedByUserId = team.CreatedByUserId,
			CreatedAt = team.CreatedAt,
			UpdatedAt = team.UpdatedAt,
			Members = team.Members.Select(member => new TeamMemberResponse
			{
				Id = member.Id,
				StudentProfileId = member.StudentProfileId,
				FullName = member.FullName,
				Email = member.Email,
				RoleInTeam = member.RoleInTeam
			}).ToList()
		});
	}
}
