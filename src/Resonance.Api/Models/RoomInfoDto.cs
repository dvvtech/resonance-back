namespace Resonance.Api.Models
{
    public class RoomInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public List<TrackDto> Tracks { get; set; } = new();
        public TrackDto? CurrentTrack { get; set; }
        public bool IsPlaying { get; set; }
        public double CurrentPosition { get; set; }
        public int ConnectedUsers { get; set; }
    }
}
