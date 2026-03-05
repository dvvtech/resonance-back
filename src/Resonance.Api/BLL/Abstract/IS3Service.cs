using Resonance.Api.BLL.Models.S3;

namespace Resonance.Api.BLL.Abstract
{
    public interface IS3Service
    {
        // Загрузка файла
        Task<S3OperationResult<string>> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string bucketName = null,
            string contentType = null,
            Dictionary<string, string> metadata = null);
    }
}
