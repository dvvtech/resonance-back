namespace Resonance.Api.Models
{
    public class RoomInfoDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public List<TrackDto> Tracks { get; set; } = new();
        public TrackDto? CurrentTrack { get; set; }
        public bool IsPlaying { get; set; }
        public double CurrentPosition { get; set; }
        public int ConnectedUsers { get; set; }
    }
}
