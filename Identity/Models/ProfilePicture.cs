namespace Identity.Models;

public class ProfilePicture
{
    public Guid Id { get; set; }

    public string? FileURI { get; set; }

    public string? FileName { get; set; }

    public string? FileExtension { get; set; }

    public string? FileType { get; set; }

    public long? FileSizeInBytes { get; set; }

    public string? FileDescription { get; set; }
}
