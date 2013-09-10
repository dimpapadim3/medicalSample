using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccess.Interfaces.Transactions
{
    public class InsertCommand : ITransactionCommands
    {
 
        [BsonIgnore]
        public Action Action { get; set; }

        public InsertCommand(Action action, List<object> entity, string fullName)
        {
            RepositoryType = fullName;
            Action = action;
            Entities = entity.ToArray();
        }

        public void Excecute()
        {
            Action();
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public IList<object> Entities { get; set; }
        public string Name { get { return "InsertEntity"; } }
        public string RepositoryType { get;   set; }
        public Func<bool> VerifyHasExecuted { get; set; }
         
    }
}