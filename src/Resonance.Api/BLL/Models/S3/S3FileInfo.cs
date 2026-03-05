namespace Resonance.Api.BLL.Models.S3
{
    public class S3FileInfo
    {
        public string Name { get; set; }
        public string BucketName { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
        public string ETag { get; set; }
        public string ContentType { get; set; }
    }
}
