namespace Resonance.Api.BLL.Abstract
{
    public interface IFileService
    {
        Task<string> UploadTrackAsync(IFormFile file, int userId);
        Task<bool> DeleteTrackAsync(string trackUrl);
    }
}
