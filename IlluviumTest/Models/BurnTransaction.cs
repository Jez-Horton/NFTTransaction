using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Models
{
    public class BurnTransaction : Transaction
    {
        public BurnTransaction()
        {
            Type = "Burn";
        }

    }
}
