//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using DataAccess.Interfaces;
//using DataAccess.Interfaces.Transactions;
//using ErrorClasses;
//using Model.DailyInfo;
//using Model.View;

//namespace Business.View
//{
//    public class UnitOfWork
//    {
//        public ITransactionRepository TransactionRepository { get; set; }

//        public void Save(Transaction transaction)
//        {
//            Transaction = transaction;
//            Transaction.TransactionCommands = new List<ITransactionCommands>();
//            transaction.Execute();

//            transaction.TransactionState = new TransactionStateInitial();
//            transaction.TransactionState.Execute();

//            Transaction.TransactionCommands.ToList().ForEach(c => c.Excecute());
//        }

//        public Transaction Transaction { get; set; }
//    }

//    public class TransactionWrapperRepository<T> : IRepositoryBase<T> where T : class
//    {
//        public UnitOfWork UnitOfWork { get; set; }

//        public IRepositoryBase<T> BaseRepository { get; set; }

//        public TransactionWrapperRepository(IRepositoryBase<T> repositoryBase)
//        {
//            BaseRepository = repositoryBase;
//        }

//        public void Remove(Expression<Func<T, bool>> filter)
//        {
//            GenericError error;
//            var func = filter.Compile();
//            UnitOfWork.Transaction.TransactionCommands.Add(new RemoveCommand(
//                () => BaseRepository.Remove(filter),
//                GetEntities(out error, func.Invoke).Cast<object>().ToList(),
//                BaseRepository.GetType().GetInterfaces().First().AssemblyQualifiedName));
//        }

//        public void Drop()
//        {
//            BaseRepository.Drop();
//        }

//        public IQueryable<T1> GetAsQueryable<T1>()
//        {
//            return BaseRepository.GetAsQueryable<T1>();
//        }

//        public string CollectionName { get { return BaseRepository.CollectionName; } }

//        public List<T> GetEntities(out GenericError error, Func<T, bool> filter = null)
//        {
//            return BaseRepository.GetEntities(out error, filter);
//        }

//        public void InsertEntity(out GenericError error, IEnumerable<T> entity)
//        {
//            var command = new InsertCommand(
//                () =>
//                {
//                    GenericError error2;
//                    BaseRepository.InsertEntity(out error2, entity);
//                }, entity.Cast<object>().ToList(), BaseRepository.GetType().GetInterfaces().First().AssemblyQualifiedName)
//                {
//                    VerifyHasExecuted = () => true
//                };

//            UnitOfWork.Transaction.TransactionCommands.Add(command);
//            error = null;
//        }

//        public void InsertEntity(out GenericError error, T entity)
//        {
//            UnitOfWork.Transaction.TransactionCommands.Add(
//                 new InsertCommand(
//                            () => { GenericError error2; BaseRepository.InsertEntity(out error2, entity); },
//                            new List<object> { entity }, BaseRepository.GetType().GetInterfaces().First().AssemblyQualifiedName));
//            error = null;

//        }
//    }

//    public class ViewUnitOfWork : UnitOfWork
//    {
//        private IRepositoryBase<TrainingSession> _trainingSessionsRepository;
//        private IRepositoryBase<TrainingSessionMeasurmentData> _trainingSessionDataRepository;

//        public ViewUnitOfWork(
//            IRepositoryBase<TrainingSession> trainingSessionsRepository1,
//            IRepositoryBase<TrainingSessionMeasurmentData> trainingSessionDataRepository1)
//        {

//            TrainingSessionsRepository = trainingSessionsRepository1;
//            TrainingSessionDataRepository = trainingSessionDataRepository1;
//        }
//        public IRepositoryBase<TrainingSession> TrainingSessionsRepository
//        {
//            get { return _trainingSessionsRepository; }
//            set
//            {
//                _trainingSessionsRepository = new TransactionWrapperRepository<TrainingSession>(value) { UnitOfWork = this };
//            }
//        }

//        public IRepositoryBase<TrainingSessionMeasurmentData> TrainingSessionDataRepository
//        {
//            get { return _trainingSessionDataRepository; }
//            set
//            {
//                _trainingSessionDataRepository = new TransactionWrapperRepository<TrainingSessionMeasurmentData>(value) { UnitOfWork = this };
//            }
//        }
//    }

//}
