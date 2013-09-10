using AspectMap;
using Business.DailyInfo;
using Business.EnergyLevelInfo;
using Business.Helpers;
using Business.Interfaces;
using Business.Questionnaire;
using Business.Team;
using Business.TrainningData;
using Business.View;
using DataAccess.Interfaces;
using Model.Chat;
using Model.DailyInfo;
using Model.EnergyLevelInfo;
using Model.View;
using NoSqlDataAccess.Entities.Chat;
using NoSqlDataAccess.Entities.DailyInfo;
using NoSqlDataAccess.Entities.EnergyLevelInfo;
using NoSqlDataAccess.Entities.View;
using SqlDataAccess.Entities.View;

namespace Business.Configuration
{
    public class IocBussinesRegistry : AspectsRegistry
    {     
        
        //http://docs.structuremap.net/ConstructorAndSetterInjection.htm

        public IocBussinesRegistry()
        {
            //For<IQuestionnaireService>().Use<EmptyQuestionnaireService>();
            For<IQuestionnaireService>().Use<QuestionnaireService>();

            For<IChatService>().Use<ChatService>();

            For<IRepositoryBase<ConverastionMessage>>().Use<ConverastionMessagesRepository>();

            SetAllProperties(y => y.OfType<IRepositoryBase<ConverastionMessage>>());
            SetAllProperties(y => y.OfType<IChatService>());


            #region EnergyLevels
            For<IEnergyLevelInfoService>().Use<EnergyLevelInfoService>();


            For<IRepositoryBase<DailyEnergyLevel>>().Use<DailyEnergyLevelRepository>();
            For<IRepositoryBase<MonthlyEnergyLevel>>().Use<MonthlyEnergyLevelRepository>();
            For<IRepositoryBase<YearlyEnergyLevel>>().Use<YearlyEnergyLevelRepository>();
            For<INoSqlFinalValuesRepositoryBase<MonthlyEnergyLevel>>().Use<FinalMonthlyEnergyLevelsRepository>();
            For<INoSqlFinalValuesRepositoryBase<YearlyEnergyLevel>>().Use<FinalYearlyyEnergyLevelsRepository>();
            For<IDataAccesHelperTemplate<MonthlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>>()
                .Use<EnergyLevelInfo.Helpers.MonthlyInfoHandlingTemplate>();

            For<IDataAccesHelperTemplate<YearlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>>()
              .Use<EnergyLevelInfo.Helpers.YearyInfoHandlingTemplate>();

 
            SetAllProperties(y => y.OfType<IRepositoryBase<DailyEnergyLevel>>());
            SetAllProperties(y => y.OfType<IRepositoryBase<MonthlyEnergyLevel>>());
            SetAllProperties(y => y.OfType<IRepositoryBase<YearlyEnergyLevel>>());
            SetAllProperties(y => y.OfType<INoSqlFinalValuesRepositoryBase<MonthlyEnergyLevel>>());
            SetAllProperties(y => y.OfType<INoSqlFinalValuesRepositoryBase<YearlyEnergyLevel>>());

            SetAllProperties(y => y.OfType<IDataAccesHelperTemplate<MonthlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>>());
            SetAllProperties(y => y.OfType<IDataAccesHelperTemplate<YearlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>>());
            #endregion

            #region DailyInfoService
            For<IDailyInfoService>().Use<DailyInfoService>();

            ForConcreteType<DailyInfoService>().Configure.SetProperty(r => r.MontlyDailyInfoHandlingTemplate = new DailyInfo.Helpers.MontlyInfoHandlingTemplate());
            ForConcreteType<DailyInfoService>().Configure.SetProperty(r => r.YearlyInfoHandlingTemplate = new DailyInfo.Helpers.YearlyInfoHandlingTemplate());

            ForConcreteType<DailyInfoService>().Configure.SetProperty(r => r.YearlyInfoSummaryDataRepository = new YearlyInfoRepository());
            ForConcreteType<DailyInfoService>().Configure.SetProperty(r => r.MonthlyInfoSummaryDataRepository = new MonthlyInfoRepository());

            ForConcreteType<DailyInfoService>().Configure.SetProperty(r => r.FinalMonthlyInfoSummaryDataRepository = new FinalMonthlyInfoRepository());
            ForConcreteType<DailyInfoService>().Configure.SetProperty(r => r.FinalYearlyDailyInfoSummaryDataRepository = new FinalYearlyInfoRepository());

            For<IDailyInfoRepository>().Use<DailyInfoRepository>();



            SetAllProperties(y => y.OfType<IDailyInfoRepository>());
            SetAllProperties(y => y.OfType<IDailyInfoService>());
            #endregion


            #region TrainingService

            For<ITrainingDataService>().Use<TrainingDataService>();

            For<IRepositoryBase<TrainingSession>>().Use<TrainingSessionsRepository>();
            For<IRepositoryBase<TrainingSessionMeasurmentData>>().Use<TrainingSessionMeasurmentDataRepository>();
            For<IRepositoryBase<TrainingSessionComment>>().Use<NoSqlDataAccess.Entities.View.TrainingSessionCommentsRepository>();

            For<IFavouriteActivityRepository>().Use<FavouriteActivityRepository>();

            SetAllProperties(y => y.OfType<ITrainingDataService>());
            SetAllProperties(y => y.OfType<IRepositoryBase<TrainingSession>>());
            SetAllProperties(y => y.OfType<IRepositoryBase<TrainingSessionMeasurmentData>>());
            SetAllProperties(y => y.OfType<IRepositoryBase<TrainingSessionComment>>());

            #endregion


            #region IViewDataService

            For<IViewDataService>().Use<ViewDataService>();

            For<IZonesRepository>().Use<ViewZonesRepository>();

            SetAllProperties(y => y.OfType<IZonesRepository>());
             

            #endregion
        }
    }
}