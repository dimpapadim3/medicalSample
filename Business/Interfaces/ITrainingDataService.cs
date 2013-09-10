using System;
using System.Collections.Generic;
using Business.Helpers;
using Model.DailyInfo;
using Model.View;
using ServiceModel.DailyInfo;

namespace Business.Interfaces
{
    public interface ITrainingDataService
    {
        IList<ServiceModel.Sport> GetPopularActivities(int userId, string locale, int numberOfFavourites);
        IList<ServiceModel.Sport> GetOtherActivities(int userId, string locale, int numberOfFavourites);

        ServiceModel.DailyInfo.TrainingDataTainingDetailsModel GetTrainingDataForPeriod(int userId,
                                                                                        int period,
                                                                                        DateTime now,
                                                                                        ServiceModel.
                                                                                            DailyInfo.
                                                                                            TrainingDataSettings
                                                                                            settings);

        IList<TrainingDataTainingDetailsModel.TrainingDataValue> GetMeasurementInfoSummary(int userId, DateTime now, PeriodTrainingDataStrategy periodTrainingDataStrategy, NumericCalculationTypeStrategy numericCalculationTypeStrategy, int numberOfRecordsToDisplay, Func<TrainingSessionMeasurmentData, IList<decimal>> getter);

        List<TrainingDataTainingDetailsModel.TrainingDataValue> GetEffortSummary(int userId, DateTime now,
                                                                                 PeriodTrainingDataStrategy periodTrainingDataStrategy, NumericCalculationTypeStrategy numericCalculationTypeStrategy, int numberOfRecordsToDisplay);

        void SubmitTrainingData(int userId, TrainingSession trainigData, DateTime now);

        //void SubmitTrainingData(int userId, int activityTypeId, int efortTypeId, string sport,
        //                        DateTime now);

        Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>> GetActivityInfoSummary(int userId, DateTime now,
                                                                                                                                       int numberOfRecordsToDisplay,
                                                                                                                                       PeriodTrainingDataStrategy periodTrainingDataStrategy,
                                                                                                                                       NumericCalculationTypeStrategy numericCalculationTypeStrategy,
                                                                                                                                       IList<Types.TrainingType> selectedActivityTypes);

        List<float> GetActivityInfoSums(int userId, DateTime now, PeriodTrainingDataStrategy periodStrategy, List<Types.TrainingType> activityTypes);
    }
}