using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Services
{
    public class NFTService : IOperations, IOwnership, IStateManagement
    {
        private Dictionary<string, string> nftOwnership = new Dictionary<string, string>();
        private string _stateFile;
        private readonly IOutputService _outputService;

        public NFTService(string? StateFile, IOutputService outputService = null)
        {

            _stateFile = StateFile ?? "nft_state.json";
            _outputService = outputService;
        }

        public void MintToken(string tokenId, string address)
        {
            if (!string.IsNullOrEmpty(tokenId) && !string.IsNullOrEmpty(address) && !nftOwnership.ContainsKey(tokenId))
            {
                nftOwnership[tokenId] = address;
                _outputService.Log($"Minted token {tokenId} to address {address}.");
            }
        }

        public void BurnToken(string tokenId)
        {
            if (!string.IsNullOrEmpty(tokenId) && nftOwnership.ContainsKey(tokenId))
            {
                nftOwnership.Remove(tokenId);
                _outputService.Log($"Burned token {tokenId}.");
            }
        }

        public void TransferToken(string tokenId, string from, string to)
        {
            if (!string.IsNullOrEmpty(tokenId) && nftOwnership.ContainsKey(tokenId) && nftOwnership[tokenId] == from)
            {
                nftOwnership[tokenId] = to;
                _outputService.Log($"Transferred token {tokenId} from {from} to {to}.");
            }
        }

        public void PrintNFTOwner(string tokenId)
        {
            if (nftOwnership.ContainsKey(tokenId))
            {
                _outputService.Log($"Token {tokenId} is owned by {nftOwnership[tokenId]}.");
            }
            else
            {
                _outputService.Log($"Token {tokenId} does not exist.");
            }
        }

        public void PrintWalletNFTs(string address)
        {
            var ownedNFTs = nftOwnership.Where(kvp => kvp.Value == address).Select(kvp => kvp.Key).ToList();
            if (ownedNFTs.Any())
            {
                _outputService.Log($"Wallet {address} owns NFTs: {string.Join(", ", ownedNFTs)}");
            }
            else
            {
                _outputService.Log($"Wallet {address} owns no NFTs.");
            }
        }

        public void ResetState()
        {
            nftOwnership.Clear();
            SaveState();
            _outputService.Log("State has been reset.");
        }

        public void LoadState()
        {
            if (File.Exists(_stateFile))
            {
                var json = File.ReadAllText(_stateFile);
                nftOwnership = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
        }
        public Dictionary<string, string> GetNFTs()
        {
            return nftOwnership;
        }

        public void SaveState()
        {
            var json = JsonConvert.SerializeObject(nftOwnership);
            File.WriteAllText(_stateFile, json);
        }
    }

}
