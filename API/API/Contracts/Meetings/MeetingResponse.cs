namespace API.Contracts.Meetings;

public class MeetingResponse
{
	public Guid Id { get; set; }
	public Guid TeamId { get; set; }
	public string TeamName { get; set; } = string.Empty;
	public Guid ProjectId { get; set; }
	public string ProjectTitle { get; set; } = string.Empty;
	public Guid? RecurrenceSeriesId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? Location { get; set; }
	public DateTime StartAt { get; set; }
	public DateTime EndAt { get; set; }
	public string? ConnectionLink { get; set; }
	public bool IsCancelled { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
