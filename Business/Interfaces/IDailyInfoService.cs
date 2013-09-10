using System;
using System.Collections.Generic;
using ErrorClasses;
using Model.DailyInfo;
using ServiceModel.DailyInfo;

namespace Business.Interfaces
{
    public interface IDailyInfoService
    {

        void InsertDailyInfo(int userId, Model.DailyInfo.DailyInfo info, DateTime? now = null);
        ServiceDailyInfoSummaryData GetDailyInfoServiceViewData(int userId, int period, DateTime now);

        bool UpdateOrInsert(out GenericError error, int userId, Model.DailyInfo.DailyInfo todayDailyInfo,
                            DateTime dateTime);

        Model.DailyInfo.DailyInfo GetTodaysData(int serverDateTime, DateTime getServerDateTime);
        ServiceDailyInfo GetDayInfoViewService(int userId, int numberOfRecords, DateTime time);
        List<DailyInfoSummaryData> GetDailyInfoViewData(int userId, int i, DateTime now);
    }
}