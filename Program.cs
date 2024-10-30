using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NFTEventProcessor
{
    class Program
    {
        private static readonly string StateFile = "nft_state.json";
        private static Dictionary<string, string> nftOwnership = new Dictionary<string, string>();

        static void Main(string[] args)
        {
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
            catch (Newtonsoft.Json.JsonException e)
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
                        MintToken(transaction["TokenId"]?.ToString(), transaction["Address"]?.ToString());
                        break;
                    case "Burn":
                        BurnToken(transaction["TokenId"]?.ToString());
                        break;
                    case "Transfer":
                        TransferToken(transaction["TokenId"]?.ToString(), transaction["From"]?.ToString(), transaction["To"]?.ToString());
                        break;
                    default:
                        Console.WriteLine("Unsupported transaction type.");
                        break;
                }
            }
        }

        static void MintToken(string tokenId, string address)
        {
            if (!string.IsNullOrEmpty(tokenId) && !string.IsNullOrEmpty(address) && !nftOwnership.ContainsKey(tokenId))
            {
                nftOwnership[tokenId] = address;
                Console.WriteLine($"Minted token {tokenId} to address {address}.");
            }
        }

        static void BurnToken(string tokenId)
        {
            if (!string.IsNullOrEmpty(tokenId) && nftOwnership.ContainsKey(tokenId))
            {
                nftOwnership.Remove(tokenId);
                Console.WriteLine($"Burned token {tokenId}.");
            }
        }

        static void TransferToken(string tokenId, string from, string to)
        {
            if (!string.IsNullOrEmpty(tokenId) && nftOwnership.ContainsKey(tokenId) && nftOwnership[tokenId] == from)
            {
                nftOwnership[tokenId] = to;
                Console.WriteLine($"Transferred token {tokenId} from {from} to {to}.");
            }
        }

        static void PrintNFTOwner(string tokenId)
        {
            if (nftOwnership.ContainsKey(tokenId))
            {
                Console.WriteLine($"Token {tokenId} is owned by {nftOwnership[tokenId]}.");
            }
            else
            {
                Console.WriteLine($"Token {tokenId} does not exist.");
            }
        }

        static void PrintWalletNFTs(string address)
        {
            var ownedNFTs = nftOwnership.Where(kvp => kvp.Value == address).Select(kvp => kvp.Key).ToList();
            if (ownedNFTs.Any())
            {
                Console.WriteLine($"Wallet {address} owns NFTs: {string.Join(", ", ownedNFTs)}");
            }
            else
            {
                Console.WriteLine($"Wallet {address} owns no NFTs.");
            }
        }

        static void ResetState()
        {
            nftOwnership.Clear();
            SaveState();
            Console.WriteLine("State has been reset.");
        }

        static void LoadState()
        {
            if (File.Exists(StateFile))
            {
                var json = File.ReadAllText(StateFile);
                nftOwnership = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
        }

        static void SaveState()
        {
            var json = JsonConvert.SerializeObject(nftOwnership);
            File.WriteAllText(StateFile, json);
        }
    }
}
