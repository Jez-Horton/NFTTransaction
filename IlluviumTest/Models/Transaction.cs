using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string TokenId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Hash { get; set; }
        public string Type { get; set; }  // "Mint", "Burn", "Transfer"
        public void GenerateHash()
        {
            var input = $"{TokenId}{Timestamp.Ticks}{new Random().Next()}";
            using var sha256 = SHA256.Create();
            Hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }

}
