using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace DataAccess.Interfaces.Transactions
{
    public class TransactionModel
    {
        public TransactionModel()
        {
            TransactionCommands = new List<TransactionCommandsModel>();
        }
        public IList<TransactionCommandsModel> TransactionCommands { get; set; }

        public string StateName { get; set; }

        public Guid TransactionId { get; set; }
        public ObjectId Id { get; set; }

    }
}