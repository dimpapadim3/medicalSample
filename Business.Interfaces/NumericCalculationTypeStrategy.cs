using System;
using System.Collections.Generic;
using System.Linq;
using Model.DailyInfo;

namespace Business.Interfaces
{
    public abstract class NumericCalculationTypeStrategy
    {
        public Func<Model.View.TrainingSessionMeasurmentData, IList<Model.View.DecimalData>> MeasurmentInfoSelector { get; set; }

        public Func<TrainingSession, decimal?> SessionInfoSelector { get; set; }

        public abstract float CalculateValue(List<TrainingSession> allSessionInTimespan);

        public float CalculateTotalEffortForActivity(List<TrainingSession> allSessionInTimespan,
                                                       Types.TrainingType trainingType)
        {
            return (float)allSessionInTimespan.Sum(si =>
                {
                    double totalEffort = 0;
                    if (si.TrainingEnumTypeId == trainingType)
                    {
                        double effort = GetEffordLevel(si);
                        totalEffort = totalEffort + effort;
                    }
                    return totalEffort;
                });
        }

        public float CalculateTotalEffort(List<TrainingSession> allSessionInTimespan)
        {
            return (float)allSessionInTimespan.Sum(si =>
                {
                    double totalEffort = 0;

                    double effort = GetEffordLevel(si);
                    totalEffort = totalEffort + effort;
                    return totalEffort;
                });
        }

        private static double GetEffordLevel(TrainingSession si)
        {
            double effort = 0;
            if (si.EffortTypeEnumId == Types.EffortType.Recovery)
                effort = 0.25;
            if (si.EffortTypeEnumId == Types.EffortType.Moderate)
                effort = 0.5;
            if (si.EffortTypeEnumId == Types.EffortType.MaxPush)
                effort = 0.75;
            if (si.EffortTypeEnumId == Types.EffortType.Hard)
                effort = 1;
            return effort;
        }
    }
 
}