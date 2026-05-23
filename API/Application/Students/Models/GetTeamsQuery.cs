namespace Application.Students.Models;

public class GetTeamsQuery
{
	public string? Search { get; set; }
	public Guid? ProjectId { get; set; }
	public TeamListFilter Filter { get; set; } = TeamListFilter.All;
	public int Offset { get; set; } = 0;
	public int Limit { get; set; } = 8;
}
