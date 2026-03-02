using Resonance.Api.BLL.Abstract;
using Microsoft.AspNetCore.SignalR;
using Resonance.Api.Models;

namespace Resonance.Api.BLL.Hub
{
    public class MusicHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<MusicHub> _logger;

        public MusicHub(IRoomService roomService, ILogger<MusicHub> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        public async Task JoinRoom(int roomId)
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null)
            {
                await Clients.Caller.SendAsync("Error", "Room not found");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            _roomService.AddUserToRoom(roomId, Context.ConnectionId);

            // Отправляем информацию о комнате новому пользователю
            var roomInfo = new RoomInfoDto
            {
                Id = room.Id,
                RoomName = room.RoomName,
                OwnerId = room.OwnerId,
                Tracks = room.Tracks.Select(t => new TrackDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Artist = t.Artist,
                    S3Url = t.S3Url
                }).ToList(),
                CurrentTrack = room.CurrentTrack != null ? new TrackDto
                {
                    Id = room.CurrentTrack.Id,
                    Title = room.CurrentTrack.Title,
                    Artist = room.CurrentTrack.Artist,
                    S3Url = room.CurrentTrack.S3Url
                } : null,
                IsPlaying = room.IsPlaying,
                CurrentPosition = room.CurrentPosition,
                ConnectedUsers = room.ConnectedUsers.Count
            };

            await Clients.Caller.SendAsync("RoomInfo", roomInfo);

            // Уведомляем других о новом пользователе
            await Clients.OthersInGroup(roomId.ToString()).SendAsync("UserJoined", room.ConnectedUsers.Count);
        }

        public async Task LeaveRoom(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
            _roomService.RemoveUserFromRoom(roomId, Context.ConnectionId);

            var room = _roomService.GetRoom(roomId);
            if (room != null)
            {
                await Clients.Group(roomId.ToString()).SendAsync("UserLeft", room.ConnectedUsers.Count);
            }
        }

        public async Task PlayTrack(int roomId, int trackId, double position = 0)
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null)
            {
                await Clients.Caller.SendAsync("Error", "Room not found");
                return;
            }

            // Только владелец может запускать воспроизведение
            if (Context.ConnectionId != room.OwnerId.ToString())
            {
                await Clients.Caller.SendAsync("Error", "Only room owner can play music");
                return;
            }

            var track = room.Tracks.FirstOrDefault(t => t.Id == trackId);
            if (track == null)
            {
                await Clients.Caller.SendAsync("Error", "Track not found");
                return;
            }

            room.CurrentTrack = track;
            room.IsPlaying = true;
            room.CurrentPosition = position;
            room.LastPlayedAt = DateTime.UtcNow;

            // Уведомляем всех в комнате о начале воспроизведения
            await Clients.Group(roomId.ToString()).SendAsync("TrackStarted", new
            {
                Track = new TrackDto
                {
                    Id = track.Id,
                    Title = track.Title,
                    Artist = track.Artist,
                    S3Url = track.S3Url
                },
                Position = position,
                StartedAt = room.LastPlayedAt
            });
        }

        public async Task PauseTrack(int roomId)
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null)
            {
                await Clients.Caller.SendAsync("Error", "Room not found");
                return;
            }

            // Только владелец может ставить на паузу
            if (Context.ConnectionId != room.OwnerId.ToString())
            {
                await Clients.Caller.SendAsync("Error", "Only room owner can pause music");
                return;
            }

            room.IsPlaying = false;
            if (room.LastPlayedAt.HasValue)
            {
                var elapsed = (DateTime.UtcNow - room.LastPlayedAt.Value).TotalSeconds;
                room.CurrentPosition += elapsed;
            }

            await Clients.Group(roomId.ToString()).SendAsync("TrackPaused", room.CurrentPosition);
        }

        public async Task SeekTrack(int roomId, double position)
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null)
            {
                await Clients.Caller.SendAsync("Error", "Room not found");
                return;
            }

            // Только владелец может перематывать
            if (Context.ConnectionId != room.OwnerId.ToString())
            {
                await Clients.Caller.SendAsync("Error", "Only room owner can seek");
                return;
            }

            room.CurrentPosition = position;
            room.LastPlayedAt = DateTime.UtcNow;

            await Clients.Group(roomId.ToString()).SendAsync("TrackSeeked", position);
        }

        public async Task SyncPosition(int roomId)
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null) return;

            double currentPosition = room.CurrentPosition;

            if (room.IsPlaying && room.LastPlayedAt.HasValue)
            {
                var elapsed = (DateTime.UtcNow - room.LastPlayedAt.Value).TotalSeconds;
                currentPosition += elapsed;
            }

            await Clients.Caller.SendAsync("SyncPosition", new
            {
                Position = currentPosition,
                IsPlaying = room.IsPlaying,
                CurrentTrack = room.CurrentTrack?.Id
            });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Ищем комнаты где был этот пользователь и удаляем его
            var rooms = _roomService.GetUserRooms(int.Parse(Context.ConnectionId));
            foreach (var room in rooms)
            {
                _roomService.RemoveUserFromRoom(room.Id, Context.ConnectionId);
                await Clients.Group(room.Id.ToString()).SendAsync("UserLeft", room.ConnectedUsers.Count);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
