using NoteFlowAPI.DTO.Conspect;
using NoteFlowAPI.Models;

namespace NoteFlowAPI.Mappers;

public static class ConspectMap
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
            DateTime = conspect.DateTime,
            UserId = conspect.UserId,
            IsPrivate = conspect.IsPrivate,
            AllowedUserIds = conspect.AllowedUserIds,
        };
    }

    public static Conspect ToConspect(this CreateConspectDTO dto)
    {
        return new Conspect
        {
            Title = dto.Title,
            Type = dto.Type,
            Description = dto.Description,
            Text = dto.Text,
            DateTime = DateTime.UtcNow,
            IsPrivate = false,
            AllowedUserIds = new List<string>(),
        };
    }

    public static void UpdateFromDto(this Conspect conspect, UpdateConspectDTO dto)
    {
        conspect.Title = dto.Title;
        conspect.Type = dto.Type;
        conspect.Description = dto.Description;
        conspect.Text = dto.Text;
    }
}

