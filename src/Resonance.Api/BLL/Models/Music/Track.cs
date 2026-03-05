namespace Resonance.Api.BLL.Models.Music
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string S3Url { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
