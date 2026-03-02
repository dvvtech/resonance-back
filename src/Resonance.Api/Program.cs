using Resonance.Api.AppStart;
using Resonance.Api.AppStart.Extensions;
using Resonance.Api.BLL.Hub;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder);
startup.Initialize();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Для локального хранения файлов

app.ApplyCors();

app.UseAuthorization();

app.MapControllers();

app.MapHub<MusicHub>("/musicHub");
// Создаем директорию для загрузок
Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "uploads"));

app.Run();
