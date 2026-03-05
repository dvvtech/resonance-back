using Microsoft.AspNetCore.Mvc;
using Resonance.Api.BLL.Abstract;
using Resonance.Api.BLL.Models;
using Resonance.Api.BLL.Models.Music;
using Resonance.Api.Models;

namespace Resonance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TracksController : ControllerBase
    {
        private readonly IFileService _s3Service;

        //todo будет БД
        private readonly List<Track> _tracks = new();

        public TracksController(IFileService s3Service)
        {
            _s3Service = s3Service;            
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadTrack(IFormFile file, [FromHeader] int userId)
        {
            var user = new User() { Id = 1, Username = "dvv" };//_authService.GetUserById(userId);
            if (user == null)
                return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Проверяем что это аудио файл
            var allowedExtensions = new[] { ".mp3", ".wav", ".ogg", ".m4a" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Only audio files are allowed.");

            var trackUrl = await _s3Service.UploadTrackAsync(file, userId);

            var track = new Track
            {
                Id = 1,
                Title = Path.GetFileNameWithoutExtension(file.FileName),
                Artist = user.Username,
                S3Url = trackUrl,
                OwnerId = userId
            };

            _tracks.Add(track);
            user.Tracks.Add(track);

            return Ok(new TrackDto
            {
                Id = track.Id,
                Title = track.Title,
                Artist = track.Artist,
                S3Url = track.S3Url
            });
        }

        [HttpGet("my-tracks")]
        public IActionResult GetMyTracks([FromHeader] string userId)
        {
            //var user = _authService.GetUserById(userId);
            //if (user == null)
            //    return Unauthorized();

            var tracks = _tracks//.Where(t => t.OwnerId == userId)
                .Select(t => new TrackDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Artist = t.Artist,
                    S3Url = t.S3Url
                }).ToList();

            return Ok(tracks);
        }

        [HttpDelete("{trackId}")]
        public async Task<IActionResult> DeleteTrack(int trackId, [FromHeader] int userId)
        {
            var track = _tracks.FirstOrDefault(t => t.Id == trackId);
            if (track == null)
                return NotFound();

            if (track.OwnerId != userId)
                return Forbid();

            await _s3Service.DeleteTrackAsync(track.S3Url);
            _tracks.Remove(track);

            return NoContent();
        }
    }
}
