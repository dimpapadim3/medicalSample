using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Business.Interfaces;
using ErrorClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.EnergyLevelInfo;
using NoSqlDataAccess.Entities.DailyInfo;
using ServiceModel;
using StructureMap;

namespace Business.Tests
{
    [TestClass]
    public class FavoriteActivitiesTest
    {


        [TestInitialize]
        public void FavoriteActivitiesTestInitialize()
        {
            IocConfigurator.Configure();
        }

        [TestMethod]
        [TestCategory(" FavouriteActivity ")]
        public void ShouldUpdateFavouriteActivityCount()
        {
            new FavouriteActivityRepository().Drop();

            var userService = ObjectFactory.GetInstance<IUserService>();
            var popularActivitiesTracking = ObjectFactory.GetInstance<IPopularActivitiesTrackingService>();


            GenericError error;
            var excludedSportIds = new long[] { 1, 5, 10, 11, 12, 13 };

            popularActivitiesTracking.UpdateFavouriteActivities("10008", 87);

            IComparer<ISport> popularSportComparer = popularActivitiesTracking.GetPopularActivitiesComparer(87);
            IEnumerable<SportExtended> sportsList =
                userService.GetMasterUserSportsExtended(87, out error)
                           .SportsList.ToList()
                           .Where(s => !excludedSportIds.Contains(s.Id));

            List<SportExtended> sorted = sportsList.ToList();
            sorted.Sort(popularSportComparer);
            Assert.IsTrue(sorted[0].Id.ToString(CultureInfo.InvariantCulture) == "10008");
            Assert.IsTrue(sorted[1].Id.ToString(CultureInfo.InvariantCulture) == "2");
        }

        [TestMethod]
        [TestCategory(" FavouriteActivity ")]
        public void ShouldUpdateFavouriteActivityForMultipleActivities()
        {
            new FavouriteActivityRepository().Drop();

            var userService = ObjectFactory.GetInstance<IUserService>();
            var popularActivitiesTracking = ObjectFactory.GetInstance<IPopularActivitiesTrackingService>();

            GenericError error;
            var excludedSportIds = new long[] { 1, 5, 10, 11, 12, 13 };

            popularActivitiesTracking.UpdateFavouriteActivities("10008", 87);
            popularActivitiesTracking.UpdateFavouriteActivities("10021", 87);

            IComparer<ISport> popularSportComparer = popularActivitiesTracking.GetPopularActivitiesComparer(87);
            IEnumerable<SportExtended> sportsList =
                userService.GetMasterUserSportsExtended(87, out error)
                           .SportsList.ToList()
                           .Where(s => !excludedSportIds.Contains(s.Id));

            List<SportExtended> sorted = sportsList.ToList();
            sorted.Sort(popularSportComparer);
            sorted = sorted.Take(3).ToList();

            Assert.IsTrue(sorted.Exists(s => s.Id.ToString(CultureInfo.InvariantCulture) == "10008"));
            Assert.IsTrue(sorted.Exists(s => s.Id.ToString(CultureInfo.InvariantCulture) == "10021"));
        }
    }
}