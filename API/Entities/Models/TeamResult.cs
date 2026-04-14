namespace Entities.Models;

public class TeamResult
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TeamId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public Guid UploadedByUserId { get; set; }
    public DateTime UploadedAt { get; set; }

    public Project? Project { get; set; }
    public Team? Team { get; set; }
    public User? UploadedByUser { get; set; }
}
