using backend.Application.Plugins.Abstract;
using backend.Application.Plugins.Concrete;
using backend.Application.Services.Abstract;
using backend.Application.Services.Concrete;
using System.Reflection;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //ScannerController.Instance.StartToScan();

            var builder = WebApplication.CreateBuilder(args);

            // Add dependency through IOC

            builder.Services.AddSingleton<IPluginProvider, PluginProvider>();
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

            app.Run();
        }
    }
}
