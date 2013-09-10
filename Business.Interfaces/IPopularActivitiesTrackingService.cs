using System.Collections.Generic;
using ServiceModel;

namespace Business.Interfaces
{
    public interface IPopularActivitiesTrackingService
    {
        void UpdateFavouriteActivities(string selectdActivityId, long userId);
        IComparer<ISport> GetPopularActivitiesComparer(int userId);
    }
}