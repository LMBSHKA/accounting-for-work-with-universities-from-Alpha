using API.Contracts.Students;
using Application.Abstractions.Students;
using Application.Students.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}
