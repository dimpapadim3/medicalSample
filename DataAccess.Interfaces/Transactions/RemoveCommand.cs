using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccess.Interfaces.Transactions
{
    public class RemoveCommand : ITransactionCommands
    {
        [BsonIgnore]
        public Action Action { get; set; }
 
        public RemoveCommand(Action entities, List<object> getEntities, string fullName)
        {
            Action = entities;
            Entities = getEntities.ToArray();
            RepositoryType = fullName;

        }

        public   void Excecute()
        { 
            Action();
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public string RepositoryType { get; set; }

        Func<bool> ITransactionCommands.VerifyHasExecuted { get; set; }
        public void VerifyAfterExecute()
        {

         }

        public IList<object> Entities { get; set; }
        public string Name { get { return "Remove"; } }
    }
}