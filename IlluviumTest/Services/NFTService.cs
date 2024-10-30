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
            ValidateTokenId(tokenId);
            ValidateAddress(address);

            if (_context.NFTs.Any(n => n.TokenId == tokenId))
                throw new InvalidOperationException($"Token {tokenId} already exists.");

            var nft = new NFT { TokenId = tokenId, OwnerAddress = address };
            var mintTransaction = new MintTransaction { TokenId = tokenId, Address = address };

            mintTransaction.GenerateHash();

            _context.NFTs.Add(nft);
            _context.Transactions.Add(mintTransaction);
            _context.SaveChanges();

            _outputService.Log($"Minted token {tokenId} to address {address}.");
        }

        public void BurnToken(string tokenId)
        {
            ValidateTokenId(tokenId);

            var nft = _context.NFTs.SingleOrDefault(n => n.TokenId == tokenId);
            if (nft == null)
            {
                _outputService.Log($"Token {tokenId} does not exist.");
                return;
            }

            var burnTransaction = new BurnTransaction { TokenId = tokenId };

            burnTransaction.GenerateHash();

            _context.NFTs.Remove(nft);

            _context.Transactions.Add(burnTransaction);
            _context.SaveChanges();

            _outputService.Log($"Burned token {tokenId}.");
        }

        public void TransferToken(string tokenId, string from, string to)
        {
            ValidateTokenId(tokenId);
            ValidateAddress(from);
            ValidateAddress(to);

            var nft = _context.NFTs.SingleOrDefault(n => n.TokenId == tokenId && n.OwnerAddress == from);
            if (nft == null)
            {
                _outputService.Log($"Token {tokenId} not owned by {from} or does not exist.");
                return;
            }

            var transferTransaction = new TransferTransaction { TokenId = tokenId, From = from, To = to };

            transferTransaction.GenerateHash();

            nft.OwnerAddress = to;

            _context.Transactions.Add(transferTransaction);
            _context.SaveChanges();

            _outputService.Log($"Transferred token {tokenId} from {from} to {to}.");
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
            _context.Transactions.RemoveRange(_context.Transactions);
            _context.SaveChanges();
            _outputService.Log("State has been reset.");
        }

        private void ValidateTokenId(string tokenId)
        {
            if (string.IsNullOrWhiteSpace(tokenId) || tokenId.Length != 42)
                throw new ArgumentException("Invalid Token ID length.");

            if (!tokenId.StartsWith("0x"))
                throw new ArgumentException("Token ID must start with '0x'.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(tokenId.Substring(2), @"\A\b[0-9a-fA-F]+\b\Z"))
                throw new ArgumentException("Token ID must be in hexadecimal format.");
        }

        private void ValidateAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address) || address.Length != 42)
                throw new ArgumentException("Invalid Address length.");

            if (!address.StartsWith("0x"))
                throw new ArgumentException("Address must start with '0x'.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(address.Substring(2), @"\A\b[0-9a-fA-F]+\b\Z"))
                throw new ArgumentException("Address must be in hexadecimal format.");
        }

        public Dictionary<string, string> GetNFTs()
        {
            return _context.NFTs.ToDictionary(n => n.TokenId, n => n.OwnerAddress);
        }
    }
}
