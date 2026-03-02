using Resonance.Api.BLL.Abstract;

namespace Resonance.Api.BLL.Services
{
    public class S3Service : IS3Service
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<S3Service> _logger;

        public S3Service(IConfiguration configuration, ILogger<S3Service> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> UploadTrackAsync(IFormFile file, string userId)
        {
            // Здесь должна быть реальная интеграция с S3
            // Для примера возвращаем локальный путь
            var fileName = $"{userId}_{DateTime.Now.Ticks}_{file.FileName}";
            var filePath = Path.Combine("wwwroot", "uploads", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }

        public async Task<bool> DeleteTrackAsync(string trackUrl)
        {
            try
            {
                var filePath = Path.Combine("wwwroot", trackUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
