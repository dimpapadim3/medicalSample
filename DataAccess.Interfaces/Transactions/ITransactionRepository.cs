using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ErrorClasses;

namespace DataAccess.Interfaces.Transactions
{
    public interface ITransactionRepository : IRepositoryBase<TransactionModel>
    {
        void UpdateTransaction(Guid Id, string stateName);
    }


  
}
