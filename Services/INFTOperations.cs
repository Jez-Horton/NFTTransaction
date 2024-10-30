using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Services
{
    public interface IOperations
    {
        void MintToken(string tokenId, string address);
        void BurnToken(string tokenId);
        void TransferToken(string tokenId, string from, string to);
    }
}
