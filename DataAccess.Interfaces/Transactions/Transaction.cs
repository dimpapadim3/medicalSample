using System;
using System.Collections.Generic;
using System.Linq;
using ErrorClasses;

namespace DataAccess.Interfaces.Transactions
{
    public class Transaction
    {
        private TransactionState _transactionState;
        public ITransactionRepository TransactionRepository { get; set; }

        public Action Action { get; set; }

        public Transaction(Action action)
        {
            Action = action;

        }

        public string StateName { get; set; }

        public IList<ITransactionCommands> TransactionCommands { get; set; }

        public TransactionState TransactionState
        {
            get { return _transactionState; }
            set
            {
                _transactionState = value;

                _transactionState.Transaction = this;
            }
        }

        public Guid TransactionId { get; set; }

        public void Execute()
        {
            Action();
        }

        public void UpdateTransactionState(TransactionState state)
        {
            TransactionRepository.UpdateTransaction(TransactionId, state.StateName);
        }

        public void InsertEntity(out  ErrorClasses.GenericError error, TransactionModel model)
        {
            TransactionRepository.InsertEntity(out error, model);
        }

        public bool VerifyExistInState(TransactionState state)
        {
            try
            {
                GenericError error;
                var exist = TransactionRepository.GetEntities(out error,
                    t => t.TransactionId == TransactionId &&
                         t.StateName == state.StateName).Any();
                return error == null && exist;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<TransactionModel> Get()
        {
            GenericError error;
            return TransactionRepository.GetEntities(out error, t => t.TransactionId == TransactionId);
        }
        public IList<TransactionModel> GetPending()
        {
            GenericError error;
            return TransactionRepository.GetEntities(out error, t => t.TransactionId == TransactionId 
                && t.StateName == "pending");
        }
    }
}
