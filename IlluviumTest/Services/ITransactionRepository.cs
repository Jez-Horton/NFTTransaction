using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IlluviumTest.Models;

namespace IlluviumTest.Services
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
    }
}
