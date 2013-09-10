using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorClasses;
using StructureMap;

namespace DataAccess.Interfaces.Transactions
{
    public abstract class TransactionState
    {

        public abstract bool VerifyInState();
        public abstract void Execute();

        public Transaction Transaction { get; set; }
        public abstract string StateName { get; }
    }

    public class TransactionStateInitial : TransactionState
    {

        private TransactionModel Convert(IEnumerable<ITransactionCommands> list)
        {
            var model = new TransactionModel() { };

            list.ToList().ForEach(c =>
                {
                    //IFormatter formatter = new BinaryFormatter();
                    //Stream stream = new MemoryStream();
                    //using (stream)
                    //{
                    //    formatter.Serialize(stream,c.Entities);
                    //    stream.Seek(0,SeekOrigin.Begin);
                    model.TransactionCommands.Add(new TransactionCommandsModel()
                     {
                         Entities = c.Entities,
                         EntityType = c.Entities.FirstOrDefault().GetType().FullName,
                         CommandType = c.Name,
                         RepositoryType = c.RepositoryType,
                         //formatter.Deserialize(stream)
                     });
                    //}

                });
            return model;
        }

        public TransactionStateInitial()
        {

        }

        public override void Execute()
        {
            Transaction.TransactionId = Guid.NewGuid();
            var model = Convert(Transaction.TransactionCommands);
            model.StateName = StateName;
            model.TransactionId = Transaction.TransactionId;
            GenericError error;
            Transaction.InsertEntity(out error, model);
            if (VerifyInState())
            {
                Transaction.TransactionState = new TransactionStatePending();
                Transaction.TransactionState.Execute();
            }
            else
            {
                new FailureScenarioBeforeExcecutingTransaction().Recover(Transaction);
            }
        }

        public override string StateName
        {
            get { return "initial"; }
        }

        public override bool VerifyInState()
        {
            return Transaction.VerifyExistInState(this);
        }

    }

    public class TransactionStatePending : TransactionState
    {
        public override bool VerifyInState()
        {
            return Transaction.VerifyExistInState(this);
        }

        public override void Execute()
        {
            var transaction = Transaction.Get();
            if (transaction == null) new FailureScenarioBeforeExcecutingTransaction().Recover(Transaction);
            Transaction.UpdateTransactionState(this);
            if (VerifyInState())
            {
                //aplu transaction 
                Transaction.TransactionState = new TransactionStateApplyChanges();
                Transaction.TransactionState.Execute();
            }
            else new FailureScenarioBeforeExcecutingTransaction().Recover(Transaction);

        }

        public override string StateName
        {
            get { return "pending"; }
        }
    }

    public class TransactionStateApplyChanges : TransactionState
    {
        public override bool VerifyInState()
        {
            var hasExecuted = true;

            Transaction.TransactionCommands.ToList().ForEach(c => hasExecuted = hasExecuted && c.VerifyHasExecuted());

            return hasExecuted;
        }

        public override void Execute()
        {
            Transaction.TransactionCommands.ToList().ForEach(c => c.Excecute());

            if (VerifyInState())
            {

            }
            var pending = Transaction.GetPending().FirstOrDefault();
            if (pending != null)
            {
              //  pending.TransactionCommands.ToList().ForEach(c => ParseCommand(c));
            }
        }

        private object ParseCommand(TransactionCommandsModel c)
        {
            Type entityType = Type.GetType(c.RepositoryType.ToString());
            var repository = ObjectFactory.GetInstance(entityType);

            if (c.CommandType == "Remove")
            {
               // var removeExpression  = Transaction.
            }
            var method = repository.GetType().GetMethod(c.CommandType);

            return new object();
        }

        public override string StateName
        {
            get { return "apply"; }
        }
    }


    public class TransactionStateCommitted
    {

    }

    public class TransactionStateDone
    {

    }

    public abstract class FailureScenario
    {
        public abstract void Recover(Transaction transaction);
    }

    public class FailureScenarioBeforeExcecutingTransaction : FailureScenario
    {
        public override void Recover(Transaction transaction)
        {

        }
    }

    public class FailureScenarioAfterExcecutingTransaction : FailureScenario
    {
        public override void Recover(Transaction transaction)
        {
        }
    }
}
