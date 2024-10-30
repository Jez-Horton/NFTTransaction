using System;
using System.IO;
using IlluviumTest.Data;
using IlluviumTest.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace NFTEventProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 23)),
                    mysqlOptions => mysqlOptions.EnableRetryOnFailure()))
                .AddScoped<IOutputService, SerilogOutputService>()
                .AddScoped<NFTService>()
                .AddScoped<CommandHandler>() 
                .BuildServiceProvider();

            try
            {
                var commandHandler = serviceProvider.GetService<CommandHandler>();
                if (args.Length > 0)
                {
                    commandHandler.ExecuteCommand(args[0], args.Length > 1 ? args[1] : null);
                }
                else
                {
                    Console.WriteLine("No command provided.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while executing the command.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
