using Minio;
using Minio.DataModel.Args;
using Resonance.Api.BLL.Abstract;
using Resonance.Api.BLL.Models.S3;

namespace Resonance.Api.BLL.Services
{
    public class S3Service : IS3Service
    {
        private readonly IMinioClient _minioClient;
        private readonly string _defaultBucket= "bucket-f2f175";
        private readonly ILogger<S3Service> _logger;

        public S3Service(
            IMinioClient minioClient,
            ILogger<S3Service> logger )
        {
            _minioClient = minioClient;
            _logger = logger;
        }

        public async Task<S3OperationResult<string>> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string bucketName = null,
            string contentType = null,
            Dictionary<string, string> metadata = null)
        {
            try
            {
                var targetBucket = bucketName ?? _defaultBucket;

                // Проверяем существование бакета
                var bucketExists = await BucketExistsAsync(targetBucket);
                if (!bucketExists)
                {
                    return S3OperationResult<string>.Fail($"Bucket not found");
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(targetBucket)
                    .WithObject(fileName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType ?? "application/octet-stream");

                // Добавляем метаданные, если есть
                if (metadata != null && metadata.Any())
                {
                    putObjectArgs.WithHeaders(metadata.ToDictionary(k => $"x-amz-meta-{k.Key}", v => v.Value));
                }

                var response = await _minioClient.PutObjectAsync(putObjectArgs);

                _logger.LogInformation("File {FileName} uploaded successfully to bucket {Bucket}. ETag: {ETag}",
                    fileName, targetBucket, response.Etag);

                return S3OperationResult<string>.Ok(response.Etag, "File uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} to bucket {Bucket}", fileName, bucketName ?? _defaultBucket);
                return S3OperationResult<string>.Fail($"Failed to upload file: {ex.Message}");
            }
        }

        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            try
            {
                var bucketExistsArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);

                return await _minioClient.BucketExistsAsync(bucketExistsArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking bucket existence {BucketName}", bucketName);
                return false;
            }
        }
    }
}
