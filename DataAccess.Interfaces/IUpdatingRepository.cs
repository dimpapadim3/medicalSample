namespace DataAccess.Interfaces
{
    public interface IUpdatingRepository<T>
    {
        bool Update(T todayDailyInfo);
    }
}
