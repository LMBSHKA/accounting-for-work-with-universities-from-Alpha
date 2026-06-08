namespace API.Contracts.Files;

public class UploadTeamFileResponse
{
    public Guid TeamId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}
