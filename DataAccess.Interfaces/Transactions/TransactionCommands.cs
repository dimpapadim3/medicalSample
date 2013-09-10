using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Interfaces.Transactions
{
    public interface ITransactionCommands
    {
        void Excecute();
        void RollBack();
        Func<bool> VerifyHasExecuted { get; set; }

        IList<object> Entities { get; set; }
        string Name { get; }
        string RepositoryType { get; set; }

    }
}
