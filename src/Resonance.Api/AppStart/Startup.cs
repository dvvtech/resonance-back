using Minio.AspNetCore;
using Resonance.Api.AppStart.Extensions;
using Resonance.Api.BLL.Abstract;
using Resonance.Api.BLL.Models.Music;
using Resonance.Api.BLL.Services;
using Resonance.Api.Configuration;

namespace Resonance.Api.AppStart
{
    public class Startup
    {
        private WebApplicationBuilder _builder;
        private readonly ILogger<Startup> _logger;

        public Startup(WebApplicationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            var loggerFactory = _builder
                .Services
                .BuildServiceProvider()
                .GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public void Initialize()
        {
            if (_builder.Environment.IsDevelopment())
            {
                _builder.Services.AddSwaggerGen();
                
            }

            InitConfigs();


            _builder.Services.ConfigureCors();
            
            // Добавляем SignalR
            _builder.Services.AddSignalR();

            RegisterServices();

            _builder.Services.AddControllers();
        }

        private void InitConfigs()
        {
            _builder.Services.Configure<S3Config>(_builder.Configuration.GetSection(S3Config.SectionName));
        }

        private void RegisterServices()
        {
            _builder.Services.AddSingleton<IFileService, FileService>();
            _builder.Services.AddSingleton<IS3Service, S3Service>();
            _builder.Services.AddSingleton<IRoomService, RoomService>();
            _builder.Services.AddSingleton<List<Track>>(); // Временное хранилище

            _builder.Services.AddMinio(options =>
            {
                var configSection = _builder.Configuration.GetSection(S3Config.SectionName);
                var config = configSection.Get<S3Config>();

                options.Endpoint = config.Endpoint;//"s3.your-provider.com";
                options.AccessKey = config.AccessKey;//"your-key";
                options.SecretKey = config.SecretKey;// "your-secret";
            });
        }
    }
}
