using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Models
{
    public abstract class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; }  // "Mint", "Burn", "Transfer"
        public string TokenId { get; set; }
    }

}
