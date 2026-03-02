namespace Resonance.Api.BLL.Models
{
    public class Room
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
        public string OwnerId { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public List<Track> Tracks { get; set; } = new();
        public Track? CurrentTrack { get; set; }
        public bool IsPlaying { get; set; }
        public double CurrentPosition { get; set; }
        public DateTime? LastPlayedAt { get; set; }
        public List<string> ConnectedUsers { get; set; } = new();
    }
}
