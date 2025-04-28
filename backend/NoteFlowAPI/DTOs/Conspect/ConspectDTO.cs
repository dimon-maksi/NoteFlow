namespace NoteFlowAPI.DTO.Conspect;

public class ConspectDTO
{
    public string? Id { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }
    public string? Description { get; set; }
    public string? Text { get; set; }
    public DateTime DateTime { get; set; }
    public bool IsPrivate { get; set; }
    public List<string>? AllowedUserIds { get; set; }
}

