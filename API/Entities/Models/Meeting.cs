namespace Entities.Models;

public class Meeting
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public DateTime StartAt { get; set; }
    public string? ConnectionLink { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Team? Team { get; set; }
    public User? CreatedByUser { get; set; }
    public ICollection<MeetingTask> Tasks { get; set; } = new List<MeetingTask>();
}
