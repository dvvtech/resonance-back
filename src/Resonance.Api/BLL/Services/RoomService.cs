using Resonance.Api.BLL.Abstract;
using Resonance.Api.BLL.Models;

namespace Resonance.Api.BLL.Services
{
    public class RoomService : IRoomService
    {
        private readonly List<Room> _rooms = new();
        private readonly ILogger<RoomService> _logger;

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
                RoomName = roomName
            };

            _rooms.Add(room);
            return room;
        }

        public Room? GetRoom(int roomId)
        {
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
