using System.Collections.Generic;
using Model.DailyInfo;

namespace DataAccess.Interfaces
{
    public interface IFavouriteActivityRepository : IRepositoryBase<FavouriteActivities> 
    {
        void UpdateRecord(KeyValuePair<string, int> updateDictionairyEntry);
    }
}