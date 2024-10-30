using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Services
{
    public interface IOwnership
    {
        void PrintNFTOwner(string tokenId);
        void PrintWalletNFTs(string address);
    }
}
