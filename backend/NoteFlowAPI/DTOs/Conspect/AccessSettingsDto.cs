namespace NoteFlowAPI.DTO.Conspect;

public class AccessSettingsDto
{
    public bool IsPrivate { get; set; }
    public List<string>? AllowedUserIds { get; set; }
}
