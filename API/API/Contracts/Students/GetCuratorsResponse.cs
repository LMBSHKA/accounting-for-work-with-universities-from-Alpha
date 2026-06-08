namespace API.Contracts.Students;

public class GetCuratorsResponse
{
	public List<CuratorListItemResponse> Items { get; set; } = [];
	public int TotalCount { get; set; }
	public int Offset { get; set; }
	public int Limit { get; set; }
	public int LoadedCount { get; set; }
	public bool HasMore { get; set; }
	public int? NextOffset { get; set; }
}
