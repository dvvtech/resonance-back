using Minio;
using Resonance.Api.BLL.Abstract;
using Resonance.Api.BLL.Models.S3;

namespace Resonance.Api.BLL.Services
{
    public class S3Service : IS3Service
    {
        private readonly IMinioClient _minioClient;

        public S3Service(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public Task<S3OperationResult<string>> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string bucketName = null,
            string contentType = null,
            Dictionary<string, string> metadata = null)
        {
            throw new NotImplementedException();
        }
    }
}
