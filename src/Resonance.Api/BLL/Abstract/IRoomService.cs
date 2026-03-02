using Resonance.Api.BLL.Models;

namespace Resonance.Api.BLL.Abstract
{
    public interface IRoomService
    {
        Room CreateRoom(string ownerId, string roomName);
        Room? GetRoom(string roomId);
        List<Room> GetUserRooms(string userId);
        bool AddTrackToRoom(string roomId, Track track);
        bool RemoveTrackFromRoom(string roomId, string trackId);
        bool SetCurrentTrack(string roomId, string trackId);
        bool AddUserToRoom(string roomId, string connectionId);
        bool RemoveUserFromRoom(string roomId, string connectionId);
    }
}
