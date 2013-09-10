using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using Model.DailyInfo;

namespace Business.TrainningData.Helpers
{
    public class MinTrainigDataSummaryStrategy : NumericCalculationTypeStrategy
    {
        public override float CalculateValue(List<TrainingSession> allSessionInTimespan)
        {
            var min = allSessionInTimespan.Min(si => AverageMeasurement(si));
            if (min != null)
                return (float)min;
            return 0;
        }

        private decimal? AverageMeasurement(TrainingSession si)
        {
            if (si.MeasurementInfo != null && MeasurmentInfoSelector(si.MeasurementInfo) != null) return MeasurmentInfoSelector(si.MeasurementInfo).Select(x => x.Value).Min();
            return 0;
        }
    }
}