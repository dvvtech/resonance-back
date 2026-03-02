using Resonance.Api.BLL.Models;

namespace Resonance.Api.BLL.Abstract
{
    public interface IRoomService
    {
        Room CreateRoom(int ownerId, string roomName);
        Room? GetRoom(int roomId);
        List<Room> GetUserRooms(int userId);
        bool AddTrackToRoom(int roomId, Track track);
        bool RemoveTrackFromRoom(int roomId, int trackId);
        bool SetCurrentTrack(int roomId, int trackId);
        bool AddUserToRoom(int roomId, string connectionId);
        bool RemoveUserFromRoom(int roomId, string connectionId);
    }
}
