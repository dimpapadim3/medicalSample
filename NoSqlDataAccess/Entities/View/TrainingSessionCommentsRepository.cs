

using System;
using System.Linq;
 
using MongoDB.Bson;
using MongoDB.Driver;

namespace NoSqlDataAccess.Entities.View
{
    public class TrainingSessionCommentsRepository : NoSqlRepositoryBase<Model.View.TrainingSessionComment>
    {
        public override string CollectionName
        {
            get
            {
                return "TrainingSessionComments";
            }
        }
    }

    //public class TransactionRepository : NoSqlRepositoryBase<TransactionModel>, ITransactionRepository
    //{
    //    public override string CollectionName
    //    {
    //        get
    //        {
    //            return "Transactions";
    //        }
    //    }


    //    public void UpdateTransaction(Guid Id, string stateName)
    //    {

    //        var userIdQuery = new QueryDocument("TransactionId", Id);
    //        var transaction = Collection.Find(userIdQuery).FirstOrDefault();
    //        if (transaction != null)
    //        {
    //            var settingBsonDoc = transaction.ToBsonDocument();
    //            settingBsonDoc["StateName"] = stateName;
    //            Collection.Save(settingBsonDoc);
    //        }

    //    }

    //}

}
