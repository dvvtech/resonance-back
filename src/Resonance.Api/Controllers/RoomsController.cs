using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resonance.Api.BLL.Abstract;
using Resonance.Api.BLL.Models;
using Resonance.Api.Models;

namespace Resonance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        private readonly List<Track> _tracks;

        public RoomsController(IRoomService roomService, List<Track> tracks)
        {
            _roomService = roomService;            
        }

        [HttpPost("create")]
        public IActionResult CreateRoom(CreateRoomDto createRoomDto, [FromHeader] int userId)
        {
            var user = new User { Id = 1 }; //_authService.GetUserById(userId);
            if (user == null)
                return Unauthorized();

            var room = _roomService.CreateRoom(userId, createRoomDto.RoomName);

            return Ok(new { room.Id, room.RoomName, ShareUrl = $"/room/{room.Id}" });
        }

        [HttpGet("{roomId}")]
        public IActionResult GetRoom(int roomId)
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null)
                return NotFound();

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
                ConnectedUsers = room.ConnectedUsers.Count
            };

            return Ok(roomInfo);
        }

        [HttpPost("{roomId}/tracks")]
        public IActionResult AddTrackToRoom(int roomId, [FromBody] int trackId, [FromHeader] int userId)
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null)
                return NotFound();

            if (room.OwnerId != userId)
                return Forbid();

            var track = _tracks.FirstOrDefault(t => t.Id == trackId);
            if (track == null)
                return NotFound("Track not found");

            if (track.OwnerId != userId)
                return Forbid("You can only add your own tracks");

            _roomService.AddTrackToRoom(roomId, track);

            return Ok(new TrackDto
            {
                Id = track.Id,
                Title = track.Title,
                Artist = track.Artist,
                S3Url = track.S3Url
            });
        }

        [HttpDelete("{roomId}/tracks/{trackId}")]
        public IActionResult RemoveTrackFromRoom(int roomId, int trackId, [FromHeader] int userId)
        {
            var room = _roomService.GetRoom(roomId);
            if (room == null)
                return NotFound();

            if (room.OwnerId != userId)
                return Forbid();

            _roomService.RemoveTrackFromRoom(roomId, trackId);

            return NoContent();
        }

        [HttpGet("my-rooms")]
        public IActionResult GetMyRooms([FromHeader] int userId)
        {
            var rooms = _roomService.GetUserRooms(userId)
                .Select(r => new
                {
                    r.Id,
                    r.RoomName,
                    r.Tracks,
                    ConnectedUsers = r.ConnectedUsers.Count
                });

            return Ok(rooms);
        }
    }
}
