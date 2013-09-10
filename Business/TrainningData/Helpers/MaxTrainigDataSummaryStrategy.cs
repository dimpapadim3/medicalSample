using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using Model.DailyInfo;

namespace Business.TrainningData.Helpers
{
    public class MaxTrainigDataSummaryStrategy : NumericCalculationTypeStrategy
    {
        public override float CalculateValue(List<TrainingSession> allSessionInTimespan)
        {
            var ret = allSessionInTimespan.Max(si => AverageMeasurement(si));
            if (ret != null)
                return (float)ret;
             return 0;
        }

        private decimal? AverageMeasurement(TrainingSession si)
        {
            if (si.MeasurementInfo != null && MeasurmentInfoSelector(si.MeasurementInfo) != null) return MeasurmentInfoSelector(si.MeasurementInfo).Select(x => x.Value).Max();
            return 0;
        }
    }
}