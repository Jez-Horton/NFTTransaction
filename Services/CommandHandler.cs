using System;
using System.IO;
using IlluviumTest.Services;
using Newtonsoft.Json.Linq;

public class CommandHandler
{
    private readonly NFTService _nftService;
    private readonly IOutputService _outputService;

    public CommandHandler(NFTService nftService, IOutputService outputService)
    {
        _nftService = nftService;
        _outputService = outputService;
    }

    public void ExecuteCommand(string command, string argument)
    {
        switch (command.ToLower())
        {
            case "--read-inline":
                ReadInline(argument);
                break;
            case "--read-file":
                ReadFile(argument);
                break;
            default:
                _outputService.Log("Unknown command.");
                break;
        }
    }

    private void ReadInline(string jsonInput)
    {
        var transactions = ParseJsonInput(jsonInput);
        ProcessTransactions(transactions);
    }

    private void ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            _outputService.Log("File not found.");
            return;
        }

        var jsonInput = File.ReadAllText(filePath);
        var transactions = ParseJsonInput(jsonInput);
        ProcessTransactions(transactions);
    }

    private JArray ParseJsonInput(string jsonInput)
    {
        try
        {
            var parsedJson = JToken.Parse(jsonInput);
            return parsedJson.Type == JTokenType.Array ? (JArray)parsedJson : new JArray { parsedJson };
        }
        catch (Newtonsoft.Json.JsonException e)
        {
            _outputService.Log($"Invalid JSON format: {e.Message}");
            return new JArray();
        }
    }

    private void ProcessTransactions(JArray transactions)
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
                    _nftService.MintToken(tokenId, address);
                    break;
                case "Burn":
                    _nftService.BurnToken(tokenId);
                    break;
                case "Transfer":
                    _nftService.TransferToken(tokenId, from, to);
                    break;
                default:
                    _outputService.Log("Unsupported transaction type.");
                    break;
            }
        }
    }
}
