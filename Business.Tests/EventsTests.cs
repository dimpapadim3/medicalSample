using System;
using Business.Interfaces;
using ErrorClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.EnergyLevelInfo;
using StructureMap;

namespace Business.Tests
{
    [TestClass]
    public class EventsTests
    {
        private const int UserId = 75;
        private readonly DateTime _now = DateTime.Now;

        [TestInitialize]
        public void EventsTestsInitialize()
        {
            IocConfigurator.Configure();
        }

        [TestMethod]
        [TestCategory(" Pop Up Events ")]
        public void PopUpEventsService_ShouldReturnTrueAfterEnergyLevelSubmited()
        {
            TestHelper.DropEnergyLevels();

            var eventsService = ObjectFactory.GetInstance<IEventsService>();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            eventsService.ClearUserDates(UserId);

            bool energyEvent;
            bool questionnaireEvent;
            bool weightEvent;
            GenericError error;

            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);

            Assert.IsTrue(energyEvent);
            energyLevelInfoService.InsertDailyEnergyLevel(new DailyEnergyLevel(UserId, _now, 1));


            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);
            Assert.IsTrue(!energyEvent);
        }

        [TestMethod]
        [TestCategory(" Pop Up Events ")]
        public void PopUpEventsService_ShouldReturnTrueAfterQuestionnaireSubmited()
        {
            TestHelper.DropQuestionnairesCollections();
            var eventsService = ObjectFactory.GetInstance<IEventsService>();

            eventsService.ClearUserDates(UserId);

            bool energyEvent;
            bool questionnaireEvent;
            bool weightEvent;
            GenericError error;

            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);

            Assert.IsTrue(questionnaireEvent);


            TestHelper.DropQuestionnairesCollections();
            TestHelper.FillNewQuestionnaireWithAnswers(ObjectFactory.GetInstance<IQuestionnaireService>(),
                                                       TestHelper.GetRandomAnswers(1), _now, UserId);

            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);
            Assert.IsTrue(!questionnaireEvent);
        }

        [TestMethod]
        [TestCategory(" Pop Up Events ")]
        public void PopUpEventsService_ShouldReturnTrueAfterEnergyLevelSubmitedAndThenQuestionnaireSubmited()
        {
            TestHelper.DropEnergyLevels();

            var eventsService = ObjectFactory.GetInstance<IEventsService>();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            eventsService.ClearUserDates(UserId);

            bool energyEvent;
            bool questionnaireEvent;
            bool weightEvent;
            GenericError error;

            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);

            Assert.IsTrue(energyEvent);
            energyLevelInfoService.InsertDailyEnergyLevel(new DailyEnergyLevel(UserId, _now, 1));


            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);
            Assert.IsTrue(!energyEvent);

            TestHelper.DropQuestionnairesCollections();
            TestHelper.FillNewQuestionnaireWithAnswers(ObjectFactory.GetInstance<IQuestionnaireService>(),
                                                       TestHelper.GetRandomAnswers(1), _now, UserId);


            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);
            Assert.IsTrue(!energyEvent);
        }

        [TestMethod]
        [TestCategory(" Pop Up Events ")]
        public void PopUpEventsService_ShouldReturnTrueAfterQuestionnaireSubmitedThenEnergyLevelSubmited()
        {
            TestHelper.DropEnergyLevels();

            var eventsService = ObjectFactory.GetInstance<IEventsService>();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            eventsService.ClearUserDates(UserId);

            bool energyEvent;
            bool questionnaireEvent;
            bool weightEvent;
            GenericError error;

            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);

            Assert.IsTrue(questionnaireEvent);


            TestHelper.DropQuestionnairesCollections();
            TestHelper.FillNewQuestionnaireWithAnswers(ObjectFactory.GetInstance<IQuestionnaireService>(),
                                                       TestHelper.GetRandomAnswers(1), _now, UserId);

            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);
            Assert.IsTrue(!questionnaireEvent);

            TestHelper.DropEnergyLevels();
            energyLevelInfoService.InsertDailyEnergyLevel(new DailyEnergyLevel(UserId, _now, 1));


            eventsService.GetPopupEvent(UserId, _now, out energyEvent, out questionnaireEvent, out weightEvent,
                                        out error);
            Assert.IsTrue(!questionnaireEvent);
        }
    }
}