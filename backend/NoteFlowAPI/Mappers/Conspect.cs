using NoteFlowAPI.DTO.Conspect;


namespace NoteFlowAPI.Mappers;

public class Conspect
{
    public static ConspectDTO ToConspectDto(this Conspect conspect)
    {
        return new ConspectDTO
        {
            Id = conspect.Id,
            Title = conspect.Title,
            Type = conspect.Type,
            Description = conspect.Description,
            Text = conspect.Text,
            DateTime = conspect.DateTime
        };
    }
}