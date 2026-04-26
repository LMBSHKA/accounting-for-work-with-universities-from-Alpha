using Entities.enums;

namespace Application.Projects.Models;

public class CreateProject
{
    public string Title { get; set; } = string.Empty;
    public string? ShortTitle { get; set; }
    public string? Description { get; set; }
    public string? Goal { get; set; }
    public ProjectStatus Status { get; set; }
    public string? Tasks { get; set; }
    public string? Mvp { get; set; }
    public Guid CreatedByUserId { get; set; }
}
