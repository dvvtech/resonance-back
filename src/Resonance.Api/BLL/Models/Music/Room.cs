namespace Resonance.Api.BLL.Models.Music
{
    public class Room
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public List<Track> Tracks { get; set; } = new();
        public Track? CurrentTrack { get; set; }
        public bool IsPlaying { get; set; }
        public double CurrentPosition { get; set; }
        public DateTime? LastPlayedAt { get; set; }
        public List<string> ConnectedUsers { get; set; } = new();
    }
}
