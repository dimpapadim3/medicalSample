namespace DataAccess.Interfaces
{
    public interface IDailyInfoRepository : IRepositoryBase<Model.DailyInfo.DailyInfo> 
    {
        bool UpdateOrInsert(Model.DailyInfo.DailyInfo todayDailyInfo);
    }
}