using System;
using System.Collections.Generic;
using Business.Interfaces;
using ErrorClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.DailyInfo;
using Model.View;
using NoSqlDataAccess.Entities.Chat;
using NoSqlDataAccess.Entities.DailyInfo;
using NoSqlDataAccess.Entities.EnergyLevelInfo;
using NoSqlDataAccess.Entities.Questionnaire;
using NoSqlDataAccess.Entities.View;
using ServiceModel.Questionnaire;
using SqlDataAccess.Entities.View;
using TrainingSessionCommentsRepository = SqlDataAccess.Entities.View.TrainingSessionCommentsRepository;

namespace Business.Tests
{
    public class TestHelper
    {
        public static void DropEnergyLevels()
        {
            new DailyEnergyLevelRepository().Drop();
            new EnergyLevelInfoRepository().Drop();
            new MonthlyEnergyLevelRepository().Drop();
            new YearlyEnergyLevelRepository().Drop();
            new FinalMonthlyEnergyLevelsRepository().Drop();
            new FinalYearlyyEnergyLevelsRepository().Drop();
        }

        public static void DropQuestionnairesCollections()
        {
            new QuestionnaireAnswersRepository().Drop();
            new YearlyQuestionnaireAnswersRepository().Drop();
            new FinalMonthlyQuestionnaireAnswersRepository().Drop();
            new MonthlyQuestionnaireAnswersRepository().Drop();
            new FinalMonthlyQuestionnaireAnswersRepository().Drop();
        }

        public static void DropDailyInfoCollections()
        {
            new DailyInfoRepository().Drop();
            new MonthlyInfoRepository().Drop();
            new YearlyInfoRepository().Drop();
            new FinalMonthlyInfoRepository().Drop();
            new FinalYearlyInfoRepository().Drop();
        }

        internal static void DropMessagesCollections()
        {
            // new ConverastionsRepository().Drop();
            new ConverastionMessagesRepository().Drop();
        }

     internal static void DropTrainningCollections()
        {
            new TrainingSessionsRepository().Drop();
            new TrainingSessionMeasurmentDataRepository().Drop();
        }

        public static void SeedTrainingData(List<int> userIds, DateTime now, int effortId, int trainigTypeId,
                                            int days = 1)
        {
            GenericError error;
            var trainingDataRepository = new TrainingSessionsRepository();
            var trainingSessionMeasurmentDataRepository = new TrainingSessionMeasurmentDataRepository();

            trainingDataRepository.Drop();
            trainingSessionMeasurmentDataRepository.Drop();

            userIds.ForEach(userId =>
                {
                    DateTime initial = now.AddDays(-days);
                    for (int i = 0; i < days; i++)
                    {
                        TrainingSession session = CreateEffortTrainingData(initial, i,
                                                                           trainingSessionMeasurmentDataRepository, null,
                                                                           userId);
                        session.TrainingTypeId = trainigTypeId;
                        session.EffortId = effortId;

                        trainingDataRepository.InsertEntity(out error, session);
                    }
                });
        }

        public static TrainingSession CreateEffortTrainingData(DateTime initial, int i,
                                                               TrainingSessionMeasurmentDataRepository
                                                                   trainingSessionMeasurmentDataRepository,
                                                               TrainingSessionCommentsRepository
                                                                   trainingSessionCommentsRepository, int userId)
        {
            GenericError error;

            DateTime currentDay = initial.AddDays(i);
            var session = new TrainingSession
                {
                    UserId = userId,
                    TrainingSessionId = Guid.NewGuid(),
                    EffortId = ((i + 2)%3 + 1),
                    TrainingTypeId = (i%6) + 1,
                    SportId = 2,
                    DateTrainingEnd = currentDay.AddHours(2),
                    DateTrainingStart = currentDay,
                };


            var sessionMeasurmentData = new TrainingSessionMeasurmentData
                {
                    TrainingSessionId = session.TrainingSessionId,
                    UserId = session.UserId,
                  
                };

            trainingSessionMeasurmentDataRepository.InsertEntity(out error, sessionMeasurmentData);


            return session;
        }


   
        private static string locale = "Questionnaire.en-US";

        public static int[] GetRandomAnswers(int i)
        {
            var answers = new[]
                {
                    new Random(i).Next(3),
                    new Random(i).Next(3),
                    new Random(i).Next(3),
                    new Random(i).Next(3),
                    new Random(i).Next(3),
                    new Random(i).Next(3),
                    new Random(i).Next(3),
                };
            return answers;
        }

        public static QuestionnaireTemplate FillNewQuestionnaireWithAnswers(IQuestionnaireService service, int[] answers,
                                                                            DateTime now, int userId)
        {
            GenericError error;
            QuestionnaireTemplate localizedTemplate = service.GetQuestionnaireTemplate(1, locale, out error,
                                                                                       DateTime.Now, userId);

            Assert.IsTrue(localizedTemplate.QuestionCategories.Count == 7);

            for (int i = 0; i < localizedTemplate.QuestionCategories.Count; i++)
            {
                QuestionCategory q = localizedTemplate.QuestionCategories[i];
                q.CategoryQuestion.QuestionAnwer = new QuestionAnwer {Answer = answers[i]};
            }

            localizedTemplate.QuestionCategories.ForEach(
                cq => cq.Questions.ForEach(q => q.QuestionAnwer = new QuestionAnwer {Answer = 1}));
            localizedTemplate.QuestionCategories[5].Questions[3].QuestionAnwer.Answer = -1;
            service.SubmitAnsweredTemplate(userId, localizedTemplate, now);
            return localizedTemplate;
        }
    }
}