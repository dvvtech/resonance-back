namespace Resonance.Api.BLL.Abstract
{
    public interface IS3Service
    {
        Task<string> UploadTrackAsync(IFormFile file, string userId);
        Task<bool> DeleteTrackAsync(string trackUrl);
    }
}
