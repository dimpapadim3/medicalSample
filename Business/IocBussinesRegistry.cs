using AspectMap;
using Business.DailyInfo;
using Business.Helpers;
using Business.Interfaces;
using DataAccess.Interfaces;
using Model.Chat;
using Model.DailyInfo;
using Model.EnergyLevelInfo;
using Model.Questionnaire;
using Model.View;
using NoSqlDataAccess.Entities.Chat;
using NoSqlDataAccess.Entities.DailyInfo;
using NoSqlDataAccess.Entities.EnergyLevelInfo;
using NoSqlDataAccess.Entities.View;
using SqlDataAccess.Entities.View;

namespace Business
{
    public class IocBussinesRegistry : AspectsRegistry
    { 
        //http://docs.structuremap.net/ConstructorAndSetterInjection.htm
        public IocBussinesRegistry()
        {
            For<IConverastionMessagesRepository>().Use<ConverastionMessagesRepository>();

            SetAllProperties(y => y.OfType<IConverastionMessagesRepository>());
            SetAllProperties(y => y.OfType<IChatService>());

            #region EnergyLevels

            For<IRepositoryBase<DailyEnergyLevel>>().Use<DailyEnergyLevelRepository>();

            For<IDataAgreggationHelperTemplate<MonthlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>>()
                .Use<EnergyLevelInfo.Helpers.MonthlyEnergyHandlingTemplateDynamic>();

            For<IDataAgreggationHelperTemplate<YearlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>>()
              .Use<EnergyLevelInfo.Helpers.YearyInfoHandlingTemplateDynamic>();


            SetAllProperties(y => y.OfType<IRepositoryBase<DailyEnergyLevel>>());
            SetAllProperties(y => y.OfType<IDataAgreggationHelperTemplate<MonthlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>>());
            SetAllProperties(y => y.OfType<IDataAgreggationHelperTemplate<YearlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>>());
            #endregion

            #region DailyInfoService


            ForConcreteType<DailyInfoService>().Configure.SetProperty(r => r.MontlyDailyInfoHandlingTemplate = new DailyInfo.Helpers.MontlyInfoAgreggationHelperTemplate());
            ForConcreteType<DailyInfoService>().Configure.SetProperty(r => r.YearlyInfoHandlingTemplate = new DailyInfo.Helpers.YearlyInfoAgreggationHelperTemplate());

            For<IDailyInfoRepository>().Use<DailyInfoRepository>();

            SetAllProperties(y => y.OfType<IDailyInfoRepository>());
            SetAllProperties(y => y.OfType<IDailyInfoService>());
            #endregion

            For<IRepositoryBase<QuestionnaireAnswers>>().Use<NoSqlDataAccess.Entities.Questionnaire.QuestionnaireAnswersRepository>();
            SetAllProperties(y => y.OfType<IRepositoryBase<QuestionnaireAnswers>>());

            #region TrainingService

            For<IRepositoryBase<TrainingSession>>().Use<SqlDataAccess.Entities.View.TrainingSessionsRepository>();            
            For<IRepositoryBase<TrainingSessionMeasurmentData>>().Use<TrainingSessionMeasurmentDataRepository>();            
            For<IRepositoryBase<TrainingSessionComment>>().Use<NoSqlDataAccess.Entities.View.TrainingSessionCommentsRepository>();
            For<IFavouriteActivityRepository>().Use<FavouriteActivityRepository>();

            SetAllProperties(y => y.OfType<ITrainingDataService>());
            SetAllProperties(y => y.OfType<IRepositoryBase<TrainingSession>>());            
            SetAllProperties(y => y.OfType<IRepositoryBase<TrainingSessionMeasurmentData>>());            
            SetAllProperties(y => y.OfType<IRepositoryBase<TrainingSessionComment>>());

            #endregion

            #region IViewDataService

            For<IZonesRepository>().Use<ViewZonesRepository>();
            SetAllProperties(y => y.OfType<IZonesRepository>());

            #endregion



            SetAllProperties(y => y.OfType<Common.IConfiguration>());
            SetAllProperties(y => y.OfType<IEnergyLevelInfoService>());
            SetAllProperties(y => y.OfType<IDailyInfoService>());
            SetAllProperties(y => y.OfType<IQuestionnaireService>());
            SetAllProperties(y => y.OfType<ITrainingDataService>());
            SetAllProperties(y => y.OfType<ITrainingSessionService>());            
            SetAllProperties(y => y.OfType<IChatService>());
            SetAllProperties(y => y.OfType<IEventsService>());

 
        }
    }
}