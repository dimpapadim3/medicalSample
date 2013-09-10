using System;
using System.Collections.Generic;
using Model.DailyInfo;
using Model.View;
using ServiceModel.DailyInfo;

namespace Business.Interfaces
{
    public interface ITrainingDataService
    {
        //  IList<ServiceModel.Sport> GetPopularActivities(int userId, string locale, int numberOfFavourites);
        //  IList<ServiceModel.Sport> GetOtherActivities(int userId, string locale, int numberOfFavourites);

        TrainingDataTainingDetailsModel GetTrainingDataForPeriod(int userId, int period, DateTime now, TrainingDataSettings settings);

        IList<TrainingDataTainingDetailsModel.TrainingDataValue> GetMeasurementInfoSummary(int userId, DateTime now, PeriodTrainingDataStrategy periodTrainingDataStrategy, NumericCalculationTypeStrategy numericCalculationTypeStrategy, int numberOfRecordsToDisplay, Func<TrainingSessionMeasurmentData, IList<DecimalData>> getter);

        List<TrainingDataTainingDetailsModel.TrainingDataValue> GetEffortSummary(int userId, DateTime now,
                                                                                 PeriodTrainingDataStrategy periodTrainingDataStrategy, NumericCalculationTypeStrategy numericCalculationTypeStrategy, int numberOfRecordsToDisplay);

        bool SubmitTrainingData(int userId, TrainingSession trainigSession);

        Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>> GetActivityInfoSummary(int userId, DateTime now,
                                                                                                                                       int numberOfRecordsToDisplay,
                                                                                                                                       PeriodTrainingDataStrategy periodTrainingDataStrategy,
                                                                                                                                       NumericCalculationTypeStrategy numericCalculationTypeStrategy,
                                                                                                                                       IList<Types.TrainingType> selectedActivityTypes);

        List<float> GetActivityInfoSums(int userId, DateTime now, PeriodTrainingDataStrategy periodStrategy, List<Types.TrainingType> activityTypes);
    }
}