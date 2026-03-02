using Resonance.Api.AppStart.Extensions;
using Resonance.Api.BLL.Abstract;
using Resonance.Api.BLL.Models;
using Resonance.Api.BLL.Services;

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
            else
            {
                _builder.Services.ConfigureCors();
            }

            // Добавляем SignalR
            _builder.Services.AddSignalR();

            RegisterServices();

            _builder.Services.AddControllers();
        }

        private void RegisterServices()
        {
            _builder.Services.AddSingleton<IS3Service, S3Service>();
            _builder.Services.AddSingleton<IRoomService, RoomService>();
            _builder.Services.AddSingleton<List<Track>>(); // Временное хранилище
        }
    }
}
