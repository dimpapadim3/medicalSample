using ErrorClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using Model;

//using Microsoft.Practices.Unity;
//using System.Threading.Tasks;


namespace Business.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TBaseModel"></typeparam>
    /// <typeparam name="TServiceModel"></typeparam>
    public abstract class DataAgreggationHelperTemplate<TModel, TBaseModel, TServiceModel>
        : IDataAgreggationHelperTemplate<TModel, TBaseModel, TServiceModel>
        where TModel : class,new()
        where TBaseModel : IUserQueryable, IDateTracable
    {
          
        public void UpdateEntityRecords(TBaseModel baseEntity)
        {
            GenericError error;
            TModel monthlyEnergyLevel = CreateNewModelItem(baseEntity);

            try
            {
                if (IsTheFirstEntryForUser(baseEntity.UserId))
                {
                    InitializeRunningTotals(monthlyEnergyLevel, baseEntity);
                }
                else
                {
                    var latestMonthlyRecord = GetLattestRecord(baseEntity.UserId);

                    if (IsTheFirstRecordOfPeriod(latestMonthlyRecord, baseEntity))
                        InitializeRunningTotals(monthlyEnergyLevel, baseEntity);
                    else
                        ComputeFromLastRecord(baseEntity, monthlyEnergyLevel, latestMonthlyRecord);

                    if (IsTheLastRecordOfthePeriod(latestMonthlyRecord, baseEntity))
                        InsertEntityToFinalRecords(out error, latestMonthlyRecord);
                }
                AddNewRecordToDataBase(monthlyEnergyLevel);
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                Controller.GetUnknownError();
                throw;
            }
        }

        public abstract bool IsTheFirstEntryForUser(long userId);
        public abstract TModel CreateNewModelItem(TBaseModel energyLevel);
        public abstract TModel GetLattestRecord(long userId);
        public abstract void InitializeRunningTotals(TModel yearlyDailyInfo, TBaseModel energyLevel);
        public abstract bool IsTheFirstRecordOfPeriod(TModel latestMonthlyRecord, TBaseModel energyLevel);
        public abstract void ComputeFromLastRecord(TBaseModel energyLevel, TModel yearlyEnergyLevel, TModel latestMonthlyRecord);
        public abstract bool IsTheLastRecordOfthePeriod(TModel latestMonthlyRecord, TBaseModel energyLevel);
        public abstract void InsertEntityToFinalRecords(out GenericError error, TModel latestMonthlyRecord);
        public abstract bool AddNewRecordToDataBase(TModel energyLevel);

        public abstract IList<TServiceModel> GetInfoView(int userid, DateTime now, int numberOfMonthsToDisplay);

        public void FIllEmptyRecords(IList<TServiceModel> intialList, int requestedRecords)
        {
            var initialCount = intialList.Count();

            if (initialCount > requestedRecords)
                return;

            Enumerable.Range(initialCount, (requestedRecords - initialCount)).ToList().ForEach(d =>
            {
                intialList.Add(CreateEmptyServiceModel());

            });
        }

        public abstract TServiceModel CreateEmptyServiceModel();

    }

}
