using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using Model.DailyInfo;

namespace Business.TrainningData.Helpers
{
    public class SumTrainigDataSummaryStrategy: NumericCalculationTypeStrategy
    {
        public override float CalculateValue(List<TrainingSession> allSessionInTimespan)
        {
            return (float)allSessionInTimespan.Sum(si => AverageMeasurement(si));
        }

        private decimal? AverageMeasurement(TrainingSession si)
        {
            if (si.MeasurementInfo != null && MeasurmentInfoSelector(si.MeasurementInfo) != null) return MeasurmentInfoSelector(si.MeasurementInfo).Select(x=> x.Value).Sum();
            return 0; 
        }
    }
}