namespace NoteFlowAPI.DTO.ConspectFilSort;

public class ConspectQueryParams
{
    public string? SearchTitle { get; set; }
    public string? SortBy { get; set; } = "title";
    public string? SortDirection { get; set; } = "asc";
}