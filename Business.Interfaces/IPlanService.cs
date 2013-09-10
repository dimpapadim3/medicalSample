using Model;
using System.Collections.Generic;

namespace Business.Interfaces
{
    public interface IPlanService
    {

        PlanDay GetPlanDay(int p1, int p2, int p3, long masterUserId, long userId);

        System.Collections.Generic.IEnumerable<PlanProfile> GetProfilesByUser(long userId);

        PlanDay GetPlanDay(long id, long userId);

        void Save(PlanDay planDay);
        void Save(PlanProfile profile);
        void Save(WaterfallUserPreferences userPreferences);
                
        //void Delete(long Id);
        void DeletePlanDays(IEnumerable<long> idsToDelete);
        void DeleteProfile(string profileId);

        IEnumerable<WaterfallUserPreferences> GetUserPreferences(long userId);

        PlanProfile GetProfile(string profileId, long userId);

    }
}