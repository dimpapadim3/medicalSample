using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace DataAccess.Interfaces.Transactions
{
    public class TransactionCommandsModel
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public object Entities { get; set; }

        public string EntityType { get; set; }

        public string CommandType { get; set; }

        public object RepositoryType { get; set; }
    }
}
