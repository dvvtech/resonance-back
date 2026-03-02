namespace Resonance.Api.BLL.Abstract
{
    public interface IS3Service
    {
        Task<string> UploadTrackAsync(IFormFile file, int userId);
        Task<bool> DeleteTrackAsync(string trackUrl);
    }
}
