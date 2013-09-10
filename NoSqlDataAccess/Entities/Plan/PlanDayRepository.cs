using ErrorClasses;
using Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Linq;

namespace NoSqlDataAccess.Entities
{
    public class PlanDayRepository : NoSqlRepositoryBase<PlanDay>
    {
        public PlanDayRepository() { }

        public override string CollectionName { get { return "PlanDays"; } }

        public PlanDay GetPlanDay(long Id)
        {
            return Collection.AsQueryable<PlanDay>().SingleOrDefault(d => d.ID.Equals(Id));
        }
        
        public PlanDay GetPlanDay(int year, int month, int day, long masterUserId) // date => 20121230
        {
            var ID = PlanDay.GetID(year, month, day, masterUserId);
            return GetPlanDay(ID);
        }

        public bool Exists(long planDayID)
        {
            return (Collection.AsQueryable<PlanDay>().FirstOrDefault(d => d.ID == planDayID) == null) ? false : true;
        }

        public PlanDay GetPlanDayByActivityId(ObjectId activityId)
        {
            return Collection.AsQueryable<PlanDay>().FirstOrDefault(d => d.Activities.Any(a => a.Id.Equals(activityId)));
        }

        public PlanDay GetPlanDayByActivityId(string activityId)
        {
            return GetPlanDayByActivityId(new ObjectId(activityId));
        }

        //public void AddActivity(Activity activity, long planDayId)
        //{
        //    var planDay = GetPlanDay(planDayId);
        //    if (planDay != null)
        //    {
        //        planDay.AddActivity(activity);
        //        Save(planDay);
        //    }
        //}

        //public void UpdateActivity(Activity activity)
        //{
        //    var planDay = Collection.AsQueryable<PlanDay>().FirstOrDefault(d => d.Activities.Any(a => a.Id.Equals(activity.Id)));

        //    if (planDay != null)
        //    {
        //        planDay.Activities.RemoveAll(a => a.Id.Equals(activity.Id));
        //        planDay.Activities.Add(activity);
        //        Save(planDay);
        //    }
        //}

        //public void DeleteActivity(ObjectId activityId)
        //{
        //    var planDay = GetPlanDayByActivityId(activityId);

        //    if (planDay != null)
        //    {
        //        planDay.Activities.RemoveAll(a => a.Id.Equals(activityId));
        //        Save(planDay);
        //    }
        //}

        //public void DeleteActivity(string activityId)
        //{
        //    var objId = new ObjectId(activityId);
        //    DeleteActivity(objId);
        //}

        public void Save(PlanDay planDay)
        {
            Collection.Save<PlanDay>(planDay);
        }

        public void DeleteDays(IEnumerable<PlanDay> days_to_delete)
        {
            var entities = Query<PlanDay>.Where(e => days_to_delete.Where(del => del != null)
                                                                   .Select(del => del.ID)
                                                                   .Contains(e.ID));
            Collection.Remove(entities);
        }

        public void DeleteDays(IEnumerable<long> ids_to_delete)
        {
            var entities = Query<PlanDay>.Where(e => ids_to_delete.Contains(e.ID));
            Collection.Remove(entities);
        }

        public void DeleteDay(long Id)
        {
            var entities = Query<PlanDay>.Where(e => e.ID.Equals(Id));
            Collection.Remove(entities);
        }


        //public void Save(PlanDay planDay)
        //{
        //    if (Exists(planDay.ID) == null)
        //    {
        //        Insert(planDay);
        //    }
        //    else
        //    {
        //        Update(planDay);
        //    }
        //}

        ///* Private Methods */
        //private void Insert(PlanDay day)
        //{
        //    GenericError error;
        //    InsertEntity(out error, day);
        //}

        //private void Update(PlanDay day)
        //{
        //    Collection.Save<PlanDay>(day);
        //}
    }
}
