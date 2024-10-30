using System;
using System.IO;
using IlluviumTest.Data;
using IlluviumTest.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace NFTEventProcessor
{
    class Program
    {
        // Entry point
        static void Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Configure logging with Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .CreateLogger();

            // Set up Dependency Injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 23)),
                    mysqlOptions => mysqlOptions.EnableRetryOnFailure()))
                .AddScoped<IOutputService, SerilogOutputService>()
                .AddScoped<NFTService>()
                .BuildServiceProvider();

            // Run the application
            try
            {
                var nftService = serviceProvider.GetService<NFTService>();
                ExecuteCommand(args, nftService);
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

        static void ExecuteCommand(string[] args, NFTService nftService)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No command provided.");
                return;
            }

            switch (args[0].ToLower())
            {
                case "--read-inline":
                    if (args.Length > 1) ProcessInlineInput(args[1], nftService);
                    else Console.WriteLine("Inline JSON input required.");
                    break;
                case "--read-file":
                    if (args.Length > 1) ProcessFileInput(args[1], nftService);
                    else Console.WriteLine("File path required.");
                    break;
                case "--nft":
                    if (args.Length > 1) nftService.PrintNFTOwner(args[1]);
                    else Console.WriteLine("Token ID required.");
                    break;
                case "--wallet":
                    if (args.Length > 1) nftService.PrintWalletNFTs(args[1]);
                    else Console.WriteLine("Wallet address required.");
                    break;
                case "--reset":
                    nftService.ResetState();
                    Console.WriteLine("State has been reset.");
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        static void ProcessInlineInput(string jsonInput, NFTService nftService)
        {
            var transactions = ParseJsonInput(jsonInput);
            ProcessTransactions(transactions, nftService);
        }

        static void ProcessFileInput(string filePath, NFTService nftService)
        {
            if (File.Exists(filePath))
            {
                var jsonInput = File.ReadAllText(filePath);
                var transactions = ParseJsonInput(jsonInput);
                ProcessTransactions(transactions, nftService);
            }
            else
            {
                Console.WriteLine("File not found.");
            }
        }

        static JArray ParseJsonInput(string jsonInput)
        {
            try
            {
                var parsedJson = JToken.Parse(jsonInput);
                return parsedJson.Type == JTokenType.Array ? (JArray)parsedJson : new JArray { parsedJson };
            }
            catch (Newtonsoft.Json.JsonException e)
            {
                Console.WriteLine($"Invalid JSON format: {e.Message}");
                return new JArray();
            }
        }

        static void ProcessTransactions(JArray transactions, NFTService nftService)
        {
            foreach (var transaction in transactions)
            {
                var type = transaction["Type"]?.ToString();
                var tokenId = transaction["TokenId"]?.ToString();
                var address = transaction["Address"]?.ToString();
                var from = transaction["From"]?.ToString();
                var to = transaction["To"]?.ToString();

                switch (type)
                {
                    case "Mint":
                        nftService.MintToken(tokenId, address);
                        break;
                    case "Burn":
                        nftService.BurnToken(tokenId);
                        break;
                    case "Transfer":
                        nftService.TransferToken(tokenId, from, to);
                        break;
                    default:
                        Console.WriteLine("Unsupported transaction type.");
                        break;
                }
            }
        }
    }
}
