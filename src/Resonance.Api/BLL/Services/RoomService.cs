using Resonance.Api.BLL.Abstract;
using Resonance.Api.BLL.Models.Music;

namespace Resonance.Api.BLL.Services
{
    public class RoomService : IRoomService
    {
        private readonly List<Room> _rooms = new();
        private readonly ILogger<RoomService> _logger;


        public RoomService()
        {
            var track = new Track
            {
                Id = 1,
                Title = "track-Title",
                Artist = "track-Artist",
                S3Url = "uploads/1_1234567890_track1.mp3"
            };

            var room = new Room
            { 
                Id = 1,
                OwnerId = 1,
                RoomName = "My room",

                Tracks = new List<Track>
                {
                    track
                },

                CurrentTrack = track
            };

            _rooms.Add(room);
        }

        public RoomService(ILogger<RoomService> logger)
        {
            _logger = logger;
        }

        public Room CreateRoom(int ownerId, string roomName)
        {
            var room = new Room
            {
                Id = 1,
                OwnerId = ownerId,
                RoomName = roomName,                
            };

            _rooms.Add(room);
            return room;
        }

        public Room? GetRoom(int roomId)
        {
            if (_rooms.Count == 0)
            {
                var track = new Track
                {
                    Id = 1,
                    Title = "track-Title",
                    Artist = "track-Artist",
                    S3Url = "http://192.168.0.47:5221/uploads/1_639080881394941769.mp3"
                    //S3Url = "uploads/1_1234567890_track1.mp3"
                };

                var room = new Room
                {
                    Id = 1,
                    OwnerId = 1,
                    RoomName = "My room",

                    Tracks = new List<Track>
                    {
                        track
                    },

                    CurrentTrack = track
                };

                _rooms.Add(room);
            }

            return _rooms.FirstOrDefault(r => r.Id == roomId);
        }

        public List<Room> GetUserRooms(int userId)
        {
            return _rooms.Where(r => r.OwnerId == userId).ToList();
        }

        public bool AddTrackToRoom(int roomId, Track track)
        {
            var room = GetRoom(roomId);
            if (room == null) return false;

            room.Tracks.Add(track);
            return true;
        }

        public bool RemoveTrackFromRoom(int roomId, int trackId)
        {
            var room = GetRoom(roomId);
            if (room == null) return false;

            var track = room.Tracks.FirstOrDefault(t => t.Id == trackId);
            if (track == null) return false;

            room.Tracks.Remove(track);

            if (room.CurrentTrack?.Id == trackId)
                room.CurrentTrack = null;

            return true;
        }

        public bool SetCurrentTrack(int roomId, int trackId)
        {
            var room = GetRoom(roomId);
            if (room == null) return false;

            var track = room.Tracks.FirstOrDefault(t => t.Id == trackId);
            if (track == null) return false;

            room.CurrentTrack = track;
            return true;
        }

        public bool AddUserToRoom(int roomId, string connectionId)
        {
            var room = GetRoom(roomId);
            if (room == null) return false;

            if (!room.ConnectedUsers.Contains(connectionId))
                room.ConnectedUsers.Add(connectionId);

            return true;
        }

        public bool RemoveUserFromRoom(int roomId, string connectionId)
        {
            var room = GetRoom(roomId);
            if (room == null) return false;

            room.ConnectedUsers.Remove(connectionId);
            return true;
        }
    }
}
