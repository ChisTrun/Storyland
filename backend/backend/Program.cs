using Backend.Application.Services.Abstract;
using Backend.Application.Services.Concrete;
using System.Reflection;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var builder = WebApplication.CreateBuilder(args);

            var pluginProvider = new PluginsScannerService();

            // Add dependency through IOC

            builder.Services.AddSingleton<IPluginsScannerService>(pluginProvider);
            builder.Services.AddSingleton<ICrawlingService, CrawlingService>();
            builder.Services.AddSingleton<IExportService, ExportService>();

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            // Razor

            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapRazorPages();

            app.Run("http://localhost:5248");
        }
    }
}
