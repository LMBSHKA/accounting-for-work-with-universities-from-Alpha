using API.Contracts.Files;
using Application.Abstractions.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/files")]
[Authorize]
public class FilesController(IWebHostEnvironment environment, IUnitOfWork unitOfWork) : ControllerBase
{
    private const long MaxFileSize = 50 * 1024 * 1024;

    private readonly IWebHostEnvironment _environment = environment;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [EndpointSummary("Загрузка файла команды")]
    [HttpPost("team/{teamId:guid}/upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<UploadTeamFileResponse>> UploadTeamFile(
        Guid teamId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (teamId == Guid.Empty)
        {
            return BadRequest(new { message = "Team id is required." });
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "File is required." });
        }

        if (file.Length > MaxFileSize)
        {
            return BadRequest(new { message = "File size must be less than or equal to 50 MB." });
        }

        var team = await _unitOfWork.Teams.GetByIdAsync(teamId, cancellationToken);
        if (team is null)
        {
            return NotFound(new { message = "Team was not found." });
        }

        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var teamUploadsPath = GetTeamUploadsPath(teamId);

        Directory.CreateDirectory(teamUploadsPath);

        var filePath = Path.Combine(teamUploadsPath, storedFileName);
        await using (var stream = new FileStream(filePath, FileMode.CreateNew))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var fileUrl = BuildTeamFileUrl(teamId, storedFileName);
        team.FileUrl = fileUrl;
        team.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Teams.Update(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(new UploadTeamFileResponse
        {
            TeamId = team.Id,
            FileName = storedFileName,
            OriginalFileName = file.FileName,
            FileUrl = fileUrl
        });
    }

    [EndpointSummary("Скачивание файла команды")]
    [HttpGet("team/{teamId:guid}/{fileName}/download")]
    public async Task<IActionResult> DownloadTeamFile(
        Guid teamId,
        string fileName,
        CancellationToken cancellationToken)
    {
        if (teamId == Guid.Empty || string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest(new { message = "Team id and file name are required." });
        }

        var safeFileName = Path.GetFileName(fileName);
        if (!string.Equals(fileName, safeFileName, StringComparison.Ordinal))
        {
            return BadRequest(new { message = "Invalid file name." });
        }

        var team = await _unitOfWork.Teams.GetByIdAsync(teamId, cancellationToken);
        if (team is null)
        {
            return NotFound(new { message = "Team was not found." });
        }

        var expectedFileUrl = BuildTeamFileUrl(teamId, safeFileName);
        if (!string.Equals(team.FileUrl, expectedFileUrl, StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { message = "File is not attached to this team." });
        }

        var filePath = Path.Combine(GetTeamUploadsPath(teamId), safeFileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { message = "File was not found." });
        }

        return PhysicalFile(filePath, "application/octet-stream", safeFileName);
    }

    private string GetTeamUploadsPath(Guid teamId)
    {
        return Path.Combine(_environment.ContentRootPath, "Storage", "Uploads", "Teams", teamId.ToString());
    }

    private static string BuildTeamFileUrl(Guid teamId, string fileName)
    {
        return $"/api/files/team/{teamId}/{fileName}/download";
    }
}
