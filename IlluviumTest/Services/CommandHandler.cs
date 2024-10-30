using System;
using System.IO;
using IlluviumTest.Services;
using Newtonsoft.Json.Linq;
using Serilog;

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
        try
        {
            switch (command.ToLower())
            {
                case "--read-inline":
                    if (string.IsNullOrWhiteSpace(argument))
                    {
                        _outputService.Log("Inline JSON input required.");
                        return;
                    }
                    ReadInline(argument);
                    break;

                case "--read-file":
                    if (string.IsNullOrWhiteSpace(argument))
                    {
                        _outputService.Log("File path required.");
                        return;
                    }
                    ReadFile(argument);
                    break;

                case "--nft":
                    if (string.IsNullOrWhiteSpace(argument))
                    {
                        _outputService.Log("Token ID required.");
                        return;
                    }
                    PrintNFTOwner(argument);
                    break;

                case "--wallet":
                    if (string.IsNullOrWhiteSpace(argument))
                    {
                        _outputService.Log("Wallet address required.");
                        return;
                    }
                    PrintWalletNFTs(argument);
                    break;

                case "--reset":
                    ResetState();
                    break;

                default:
                    _outputService.Log("Unknown command.");
                    break;
            }
        }
        catch (ArgumentException ex)
        {
            _outputService.Log($"Input error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _outputService.Log($"Transaction error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _outputService.Log($"An unexpected error occurred: {ex.Message}");
            Log.Error(ex, "Unhandled exception in command execution.");
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

    private void PrintNFTOwner(string tokenId)
    {
        _nftService.PrintNFTOwner(tokenId);
    }

    private void PrintWalletNFTs(string address)
    {
        _nftService.PrintWalletNFTs(address);
    }

    private void ResetState()
    {
        _nftService.ResetState();
        _outputService.Log("State has been reset.");
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
