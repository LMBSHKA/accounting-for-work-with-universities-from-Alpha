using Application.Abstractions.Persistence;
using Application.Abstractions.Projects;
using Application.Projects.Models;
using Entities.Models;
using Entities.enums;

namespace Application.Projects.Services;

public class ProjectService(IUnitOfWork unitOfWork) : IProjectService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Project> CreateAsync(CreateProject newProject, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Title = NormalizeRequired(newProject.Title),
            ShortTitle = NormalizeOptional(newProject.ShortTitle),
            Description = NormalizeOptional(newProject.Description),
            Goal = NormalizeOptional(newProject.Goal),
            Status = newProject.Status,
            Tasks = NormalizeOptional(newProject.Tasks),
            Mvp = NormalizeOptional(newProject.Mvp),
            CreatedByUserId = newProject.CreatedByUserId,
            CreatedAt = now,
            ApprovedAt = newProject.Status == ProjectStatus.Active ? now : null,
            ArchivedAt = newProject.Status == ProjectStatus.Archived ? now : null
        };

        var statusHistory = new ProjectStatusHistory
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            Status = project.Status,
            ChangedByUserId = newProject.CreatedByUserId,
            ChangeComment = "Проект создан",
            ChangedAt = now
        };

        await _unitOfWork.Projects.AddAsync(project, cancellationToken);
        await _unitOfWork.ProjectStatusHistory.AddAsync(statusHistory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return project;
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
