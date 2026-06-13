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
            Tasks = NormalizeOptional(newProject.EvaluationCriteria) ?? NormalizeOptional(newProject.Tasks),
            Mvp = NormalizeOptional(newProject.Mvp),
            Deadline = newProject.Deadline,
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

	public Task<GetProjectsResult> GetProjectsAsync(GetProjectsQuery request, CancellationToken cancellationToken = default)
	{
		return _unitOfWork.Projects.GetProjectsAsync(request, cancellationToken);
	}

	public async Task<bool> CompleteProjectAsync(Guid projectId, Guid changedByUserId, CancellationToken cancellationToken = default)
	{
		var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
		if (project is null || changedByUserId == Guid.Empty)
		{
			return false;
		}

		if (project.Status == ProjectStatus.Completed)
		{
			return true;
		}

		var now = DateTime.UtcNow;
		project.Status = ProjectStatus.Completed;
		project.UpdatedAt = now;
		_unitOfWork.Projects.Update(project);

		await _unitOfWork.ProjectStatusHistory.AddAsync(new ProjectStatusHistory
		{
			Id = Guid.NewGuid(),
			ProjectId = project.Id,
			Status = ProjectStatus.Completed,
			ChangedByUserId = changedByUserId,
			ChangeComment = "Проект завершен",
			ChangedAt = now
		}, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> UpdateStatusAsync(UpdateProjectStatusCommand command, CancellationToken cancellationToken = default)
	{
		if (command.ProjectId == Guid.Empty || command.ChangedByUserId == Guid.Empty || !Enum.IsDefined(command.Status))
		{
			return false;
		}

		var project = await _unitOfWork.Projects.GetByIdAsync(command.ProjectId, cancellationToken);
		if (project is null)
		{
			return false;
		}

		var now = DateTime.UtcNow;
		project.Status = command.Status;
		project.UpdatedAt = now;

		if (command.Status == ProjectStatus.Active && project.ApprovedAt is null)
		{
			project.ApprovedAt = now;
		}

		if (command.Status == ProjectStatus.Archived && project.ArchivedAt is null)
		{
			project.ArchivedAt = now;
		}

		_unitOfWork.Projects.Update(project);

		await _unitOfWork.ProjectStatusHistory.AddAsync(new ProjectStatusHistory
		{
			Id = Guid.NewGuid(),
			ProjectId = project.Id,
			Status = command.Status,
			ChangedByUserId = command.ChangedByUserId,
			ChangeComment = null,
			ChangedAt = now
		}, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<IReadOnlyCollection<ProjectStatusHistoryResult>?> GetStatusHistoryAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
		if (project is null)
		{
			return null;
		}

		return await _unitOfWork.Projects.GetStatusHistoryAsync(projectId, cancellationToken);
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
