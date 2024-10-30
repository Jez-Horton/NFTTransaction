using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Models
{
    public class MintTransaction : Transaction
    {
        public string Address { get; set; }

        public MintTransaction()
        {
            Type = "Mint";
        }
    }

}
