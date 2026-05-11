namespace Application.Students.Models;

public class GetStudentsQuery
{
	public string? Search { get; set; }
	public StudentListFilter Filter { get; set; } = StudentListFilter.All;
	public int Offset { get; set; } = 0;
	public int Limit { get; set; } = 8;
}
