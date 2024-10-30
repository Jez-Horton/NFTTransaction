using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IlluviumTest.Data;
using IlluviumTest.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace NFTEventProcessor
{
    class Program
    {

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 23))));
        }

        private static readonly string StateFile = "nft_state.json";
        private static NFTService _nftService;

        static void Main(string[] args)
        {
            // Initialize Serilog for logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            // Create the NFTService with Serilog as the output service
            _nftService = new NFTService(StateFile, new SerilogOutputService());

            LoadState();

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "--read-inline":
                        ProcessInlineInput(args[1]);
                        break;
                    case "--read-file":
                        ProcessFileInput(args[1]);
                        break;
                    case "--nft":
                        PrintNFTOwner(args[1]);
                        break;
                    case "--wallet":
                        PrintWalletNFTs(args[1]);
                        break;
                    case "--reset":
                        ResetState();
                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
                SaveState();
            }
            else
            {
                Console.WriteLine("No command provided.");
            }

            Log.CloseAndFlush(); // Ensure logging is flushed
        }

        static void ProcessInlineInput(string jsonInput)
        {
            var transactions = ParseJsonInput(jsonInput);
            ProcessTransactions(transactions);
        }

        static void ProcessFileInput(string filePath)
        {
            if (File.Exists(filePath))
            {
                var jsonInput = File.ReadAllText(filePath);
                var transactions = ParseJsonInput(jsonInput);
                ProcessTransactions(transactions);
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
            catch (JsonException e)
            {
                Console.WriteLine($"Invalid JSON format: {e.Message}");
                return new JArray();
            }
        }

        static void ProcessTransactions(JArray transactions)
        {
            foreach (var transaction in transactions)
            {
                var type = transaction["Type"]?.ToString();
                switch (type)
                {
                    case "Mint":
                        _nftService.MintToken(transaction["TokenId"]?.ToString(), transaction["Address"]?.ToString());
                        break;
                    case "Burn":
                        _nftService.BurnToken(transaction["TokenId"]?.ToString());
                        break;
                    case "Transfer":
                        _nftService.TransferToken(transaction["TokenId"]?.ToString(), transaction["From"]?.ToString(), transaction["To"]?.ToString());
                        break;
                    default:
                        Console.WriteLine("Unsupported transaction type.");
                        break;
                }
            }
        }

        static void PrintNFTOwner(string tokenId)
        {
            _nftService.PrintNFTOwner(tokenId);
        }

        static void PrintWalletNFTs(string address)
        {
            _nftService.PrintWalletNFTs(address);
        }

        static void ResetState()
        {
            _nftService.ResetState();
            Console.WriteLine("State has been reset.");
        }

        static void LoadState()
        {
            if (File.Exists(StateFile))
            {
                _nftService.LoadState();
            }
        }

        static void SaveState()
        {
            var nftOwnership = _nftService.GetNFTs();
            var json = JsonConvert.SerializeObject(nftOwnership);
            File.WriteAllText(StateFile, json);
        }
    }
}
