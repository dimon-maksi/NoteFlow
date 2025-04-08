namespace NoteFlowAPI.DTO.Conspect;

public class UpdateConspectDTO
{
    public required string Title { get; set; }
    public required string Type { get; set; }
    public required string Description { get; set; }
    public required string Text { get; set; }
}