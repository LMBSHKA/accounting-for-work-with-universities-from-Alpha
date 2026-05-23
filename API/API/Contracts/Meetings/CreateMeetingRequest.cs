using Entities.enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Meetings;

public class CreateMeetingRequest
{
	[Required]
	public Guid TeamId { get; set; }

	[MaxLength(250)]
	public string? Title { get; set; }

	[MaxLength(4000)]
	public string? Description { get; set; }

	[MaxLength(500)]
	public string? Location { get; set; }

	[Required]
	public DateTime StartAt { get; set; }

	[Required]
	public DateTime EndAt { get; set; }

	[MaxLength(1024)]
	public string? ConnectionLink { get; set; }

	[Description("Тип повторения встречи: 1 — без повторения, 2 — каждую неделю.")]
	public MeetingRepeatType RepeatType { get; set; } = MeetingRepeatType.None;

	[Description("Дни недели для еженедельного повторения. Если не передать, используется день недели StartAt.")]
	public List<DayOfWeek> DaysOfWeek { get; set; } = [];

	[Description("Конечная дата повторений включительно.")]
	public DateOnly? EndOn { get; set; }

	[Range(1, 200)]
	[Description("Всего встреч, которые нужно создать. Максимум 200.")]
	public int? OccurrencesCount { get; set; }
}
