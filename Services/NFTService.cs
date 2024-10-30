using IlluviumTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IlluviumTest.Data;

namespace IlluviumTest.Services
{
    public class NFTService : IOperations, IOwnership, IStateManagement
    {
        private readonly ApplicationDbContext _context;
        private readonly IOutputService _outputService;

        public NFTService(ApplicationDbContext context, IOutputService outputService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "ApplicationDbContext cannot be null");
            _outputService = outputService ?? throw new ArgumentNullException(nameof(outputService), "OutputService cannot be null");
        }

        public void MintToken(string tokenId, string address)
        {
            if (!string.IsNullOrEmpty(tokenId) && !string.IsNullOrEmpty(address) && !_context.NFTs.Any(n => n.TokenId == tokenId))
            {
                var nft = new NFT { TokenId = tokenId, OwnerAddress = address };
                _context.NFTs.Add(nft);
                _context.SaveChanges();
                _outputService.Log($"Minted token {tokenId} to address {address}.");
            }
        }

        public void BurnToken(string tokenId)
        {
            var nft = _context.NFTs.SingleOrDefault(n => n.TokenId == tokenId);
            if (nft != null)
            {
                _context.NFTs.Remove(nft);
                _context.SaveChanges();
                _outputService.Log($"Burned token {tokenId}.");
            }
        }

        public void TransferToken(string tokenId, string from, string to)
        {
            var nft = _context.NFTs.SingleOrDefault(n => n.TokenId == tokenId && n.OwnerAddress == from);
            if (nft != null)
            {
                nft.OwnerAddress = to;
                _context.SaveChanges();
                _outputService.Log($"Transferred token {tokenId} from {from} to {to}.");
            }
        }

        public void PrintNFTOwner(string tokenId)
        {
            var nft = _context.NFTs.SingleOrDefault(n => n.TokenId == tokenId);
            if (nft != null)
            {
                _outputService.Log($"Token {tokenId} is owned by {nft.OwnerAddress}.");
            }
            else
            {
                _outputService.Log($"Token {tokenId} does not exist.");
            }
        }

        public void PrintWalletNFTs(string address)
        {
            var ownedNFTs = _context.NFTs
                .Where(n => n.OwnerAddress == address)
                .Select(n => n.TokenId)
                .ToList();

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
            _context.NFTs.RemoveRange(_context.NFTs);
            _context.SaveChanges();
            _outputService.Log("State has been reset.");
        }

        public void LoadState()
        {
            // With EF Core, loading is done via querying. No action required here for loading as all data is in the database.
        }

        public Dictionary<string, string> GetNFTs()
        {
            return _context.NFTs.ToDictionary(n => n.TokenId, n => n.OwnerAddress);
        }

        public void SaveState()
        {
            // With EF Core, saving is done automatically with each call to _context.SaveChanges().
        }
    }
}
