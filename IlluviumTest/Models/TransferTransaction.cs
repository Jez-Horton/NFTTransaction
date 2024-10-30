using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Models
{
    public class TransferTransaction : Transaction
    {
        public string From { get; set; }
        public string To { get; set; }

        public TransferTransaction()
        {
            Type = "Transfer";
        }
    }

}
