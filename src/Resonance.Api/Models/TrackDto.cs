namespace Resonance.Api.Models
{
    public class TrackDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string S3Url { get; set; } = string.Empty;
    }
}
