using System;
using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using Common;
using ErrorClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Questionnaire;
using StructureMap;
using QuestionAnwer = ServiceModel.Questionnaire.QuestionAnwer;
using QuestionCategory = ServiceModel.Questionnaire.QuestionCategory;
using QuestionnaireTemplate = ServiceModel.Questionnaire.QuestionnaireTemplate;
using Business.Questionnaire;

namespace Business.Tests
{
    /// <summary>
    ///     Summary description for QuestionnaireTest
    /// </summary>
    [TestClass]
    public class QuestionnaireTests
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }
  
        #region Initialize

        [TestInitialize]
        public void TestInitialize()
        {
            IocConfigurator.Configure();
        }

        #endregion


        [TestMethod]
        public void TestCategoryScoring()
        {
            GenericError error;
            const int userId = 75;
            var now = DateTime.Now;
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();

            TestHelper.DropQuestionnairesCollections();
            QuestionnaireTemplate localizedTemplate = service.GetQuestionnaireTemplate(2, Constants.DEFAULT_QUESTIONNAIRE_LOCALE, out error, now, userId);

            foreach (QuestionCategory q in localizedTemplate.QuestionCategories){q.CategoryQuestion.QuestionAnwer = new QuestionAnwer { Answer = 1 };}

            localizedTemplate.QuestionCategories.ForEach(cq => cq.Questions.ForEach(q => q.QuestionAnwer = new QuestionAnwer { Answer = 1 }));

      
        }




        [TestMethod]
        public void ShouldGetTheTemplate()
        {
            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();
 
                TestHelper.DropQuestionnairesCollections();
                QuestionnaireTemplate localizedTemplate = service.GetQuestionnaireTemplate(2, Constants.DEFAULT_QUESTIONNAIRE_LOCALE, out error, DateTime.Now,75);
             
          
        }
        [TestMethod]
        public void ShouldSubmitTemplate()
        {
            GenericError error;
            int userId = 75;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();

            TestHelper.DropQuestionnairesCollections();
            int[] target = TestHelper.GetRandomAnswers(1);
            TestHelper.DropQuestionnairesCollections();

            QuestionnaireTemplate localizedTemplate = service.GetQuestionnaireTemplate(1, Constants.DEFAULT_QUESTIONNAIRE_LOCALE, out error, DateTime.Now, 75);

            for (int i = 0; i < localizedTemplate.QuestionCategories.Count; i++)
            {
                QuestionCategory q = localizedTemplate.QuestionCategories[i];
                q.CategoryQuestion.QuestionAnwer = new QuestionAnwer { Answer = 1 };
            }

            localizedTemplate.QuestionCategories.ForEach(
                cq => cq.Questions.ForEach(q => q.QuestionAnwer = new QuestionAnwer { Answer = 1 }));
            

            service.SubmitAnsweredTemplate(userId, localizedTemplate, now);

              localizedTemplate = service.GetQuestionnaireTemplate(3, Constants.DEFAULT_QUESTIONNAIRE_LOCALE, out error, DateTime.Now.AddDays(-1), 75);

            for (int i = 0; i < localizedTemplate.QuestionCategories.Count; i++)
            {
                QuestionCategory q = localizedTemplate.QuestionCategories[i];
                q.CategoryQuestion.QuestionAnwer = new QuestionAnwer { Answer = 1 };
            }

            localizedTemplate.QuestionCategories.ForEach(
                cq => cq.Questions.ForEach(q => q.QuestionAnwer = new QuestionAnwer { Answer = 1 }));


            service.SubmitAnsweredTemplate(userId, localizedTemplate, DateTime.Now.AddDays(-1));
           
        }


        [TestMethod]
        public void ShouldGetTheCorrectDayData()
        {
            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();

            for (int i = 0; i < 100; i++)
            {
                int[] target = TestHelper.GetRandomAnswers(i);
                TestHelper.DropQuestionnairesCollections();
                QuestionnaireTemplate localizedTemplate = TestHelper.FillNewQuestionnaireWithAnswers(service, target, now, userId);
                List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                              Constants
                                                                                                                  .DAY_PERIOD,
                                                                                                              now);

                for (int j = 0; j < actualDayData.Count(); j++)
                    Assert.IsTrue(target[j + 1] == (int) actualDayData[j].Answer);
            }
        }

        #region Week Average Questionnaire

        [TestMethod]
        public void ShouldGetTheCorrectWeeklyyData_WhenTwoQuestionnairesSubmitedFiveWeeksApart()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 26);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();

            now = new DateTime(2013, 04, 12);
            var target1 = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target1, now, userId);

            var target2 = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target2, now.AddDays(-35), userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .WEEK_PERIOD,
                                                                                                          now);

            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(targetData[j + 1] == (int) actualDayData[j].Answer);
        }

        [TestMethod]
        public void ShouldFailtWeeklyyData_WhenTwoQuestionnairesSubmitedMoreThanFiveWeeksApart()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 26);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();

            now = new DateTime(2013, 04, 12);
            var target1 = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target1, now, userId);

            var target2 = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target2, now.AddDays(-36), userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .WEEK_PERIOD,
                                                                                                          now);

            var targetData = new[] {1, 1, 1, 1, 1, 1, 1};
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(targetData[j + 1] == (int) actualDayData[j].Answer);
        }

        [TestMethod]
        public void ShouldGetTheCorrectWeeklyyData()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 26);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();

            now = new DateTime(2013, 04, 12);
            var target1 = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target1, now, userId);

            var target2 = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target2, now.AddDays(-35), userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .WEEK_PERIOD,
                                                                                                          now);

            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(targetData[j + 1] == (int) actualDayData[j].Answer);
        }

 
        [TestMethod]
        public void ShouldGetTheCorrectWeeklyQuestioannaireOneDayOutOfBoundaries()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 26);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();

            var firstData = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, firstData,
                                            now.AddDays(-Constants.QUESTIONNAIRE_NUMBER_OF_WEEKS_RECORDS).AddDays(-1),
                                            userId);

            DateTime secondDate = now.AddDays(-Constants.QUESTIONNAIRE_NUMBER_OF_WEEKS_RECORDS);
            var secondData = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, secondData, secondDate, userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .WEEK_PERIOD,
                                                                                                          now);
            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};

            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsFalse(targetData[j + 1] == (int) actualDayData[j].Answer);

            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(secondData[j + 1] == (int) actualDayData[j].Answer);
        }

        #endregion

        #region Month Average Questionnaire

        [TestMethod]
        public void ShouldGetTheCorrectMonthlyyData()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();


            var target1 = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target1, now.AddDays(-1), userId);

            var target2 = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target2, now, userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .MONTH_PERIOD,
                                                                                                          now);

            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(targetData[j + 1] == (int) actualDayData[j].Answer);
        }

        [TestMethod]
        public void ShouldGetTheCorrectMonthlyQuestioannaireAtBoundaries()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();


            var target1 = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target1, now.AddMonths(-Constants.NUMBER_OF_MONTH_RECORDS), userId);

            var fiveMonthsEarlier = new DateTime(2013, 04, 25);
            var target2 = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target2, now, userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .MONTH_PERIOD,
                                                                                                          fiveMonthsEarlier);

            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(targetData[j + 1] == (int) actualDayData[j].Answer);
        }

        [TestMethod]
        public void ShouldGetTheCorrectMonthlyQuestioannaireOneDayOutOfBoundaries()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();


            var firstData = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, firstData, now.AddMonths(-Constants.NUMBER_OF_MONTH_RECORDS - 1),
                                            userId);

            var fiveMonthsEarlier = new DateTime(2013, 04, 25);
            var secondData = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, secondData, now, userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .MONTH_PERIOD,
                                                                                                          fiveMonthsEarlier);

            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};

            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsFalse(targetData[j + 1] == (int) actualDayData[j].Answer);
             
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(secondData[j + 1] == (int) actualDayData[j].Answer);
        }

        #endregion

        #region Year Average Questionnaire

        [TestMethod]
        public void ShouldGetTheCorrectYearlyData()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();


            var target1 = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target1, now.AddDays(-1), userId);

            var target2 = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target2, now, userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .YEAR_PERIOD,
                                                                                                          now);

            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(targetData[j + 1] == (int) actualDayData[j].Answer);
        }

        [TestMethod]
        public void ShouldGetTheCorrectYearyQuestioannaireAtBoundaries()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();


            var target1 = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target1, now.AddYears(-Constants.NUMBER_OF_YEAR_RECORDS), userId);

            var fiveMonthsEarlier = new DateTime(2013, 04, 25);
            var target2 = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, target2, now, userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .YEAR_PERIOD,
                                                                                                          fiveMonthsEarlier);

            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(targetData[j + 1] == (int) actualDayData[j].Answer);
        }

        [TestMethod]
        public void ShouldGetTheCorrectYearyQuestioannaireOneDayOutOfBoundaries()
        {
            TestHelper.DropQuestionnairesCollections();

            GenericError error;
            int userId = 50;
            var now = new DateTime(2013, 04, 25);
            var service = ObjectFactory.GetInstance<IQuestionnaireService>();


            var firstData = new[] {1, 1, 1, 1, 1, 1, 1};
            TestHelper.FillNewQuestionnaireWithAnswers(service, firstData, now.AddYears(-Constants.NUMBER_OF_YEAR_RECORDS - 1),
                                            userId);

            var fiveMonthsEarlier = new DateTime(2013, 04, 25);
            var secondData = new[] {3, 3, 3, 3, 3, 3, 3};
            TestHelper.FillNewQuestionnaireWithAnswers(service, secondData, now, userId);

            List<QuestionnaireAverageSummaryData> actualDayData = service.GetAverageQuestionnaireViewData(userId,
                                                                                                          Constants
                                                                                                              .YEAR_PERIOD,
                                                                                                          fiveMonthsEarlier);

            var targetData = new[] {2, 2, 2, 2, 2, 2, 2};

            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsFalse(targetData[j + 1] == (int) actualDayData[j].Answer);

            //
            for (int j = 0; j < actualDayData.Count(); j++)
                Assert.IsTrue(secondData[j + 1] == (int) actualDayData[j].Answer);
        }

        #endregion
    }
}