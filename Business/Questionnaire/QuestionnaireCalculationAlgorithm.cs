using System;
using System.Collections.Generic;
using System.Linq;
using Business.EnergyLevelInfo;
using Common;
using Model.Questionnaire;
using ServiceModel.Questionnaire;
using Question = ServiceModel.Questionnaire.Question;
using QuestionCategory = ServiceModel.Questionnaire.QuestionCategory;
using QuestionnaireTemplate = ServiceModel.Questionnaire.QuestionnaireTemplate;

namespace Business.Questionnaire
{
    public abstract class QuestionnaireCalculationAlgorithmBase : IQuestionnaireElementsCalculationAlgorithm, IQuestionnaireCoreScoreCalculationAlgorithm
    { 
        protected static double Epsilon { get { return 0.01; } }

        public virtual string GetSingleCategoryScoreColor(double score)
        {
            if (Math.Abs(score - 0) < Epsilon)
                return Constants.Questioannaire.RED_COLOR;
            if (Math.Abs(score - 1) < Epsilon)
                return Constants.Questioannaire.ORANGE_COLOR;
            if (Math.Abs(score - 2) < Epsilon)
                return Constants.Questioannaire.GREEN_COLOR;
            if (Math.Abs(score - 2.5) < Epsilon)
                return Constants.Questioannaire.GREEN_COLOR;
            return Constants.Questioannaire.LIGHT_GRAY;
        }
         
        public virtual CoreScoreAnswer AverageCoreScoreForPeriod(string locale, IList<QuestionnaireAnswers> questionnaireAnswerses)
        {
            if (questionnaireAnswerses.Any())
            {
                var averageCoreScores = (int)Math.Round(((float)questionnaireAnswerses.Sum(c => c.CoreScore) / questionnaireAnswerses.Count()));
                return GetLocalizedCoreScore(locale, (byte)averageCoreScores);
            }
            return GetNullCoreScore();
        }
         
        public virtual void CalculateSingleQuestionnaireCategoryScores(QuestionnaireAnswers questionnaireAnwsers)
        {
            questionnaireAnwsers.QuestionnaireSummaryData = new QuestionnaireSummaryData
            {
                Value = questionnaireAnwsers.CategoryQuestionsAnswers.Select(c => c.Answer).ToList()
            };
        }

        public virtual QuestionnaireSummaryData CalculateSingleQuestionnaireCategoryScores(QuestionnaireTemplate answeredTemplate)
        {
            var summaries = new QuestionnaireSummaryData { };

            for (int p = 0; p < answeredTemplate.QuestionCategories.Count; p++)
            {
                var sum = answeredTemplate.QuestionCategories[p].Questions.Sum(q => q.QuestionAnwer.Answer);

                summaries.Value.Add(sum);
            }

            return summaries;
        }

        public abstract void CalculateSingleQuestionnaireCategoryColors(QuestionnaireTemplate answeredTemplate);

        #region Core Score

        public virtual void CalculateSingleQuestionnaireCoreScoreColor(QuestionnaireTemplate instanciatedTemplate)
        {
            var overalCoreScoreType = OveralCoreScoreType(instanciatedTemplate);

            instanciatedTemplate.CoreScoreAnswer = GetLocalizedCoreScore(Constants.DEFAULT_QUESTIONNAIRE_LOCALE, overalCoreScoreType);
        }

        public int OveralCoreScoreType(QuestionnaireTemplate instanciatedTemplate)
        {
            var ratedCategoriesScores = CalculateSingleQuestionnaireCategoryScores(instanciatedTemplate);

            var totalSumOfAllCategoryElements = ratedCategoriesScores.Value.Sum();

            var overalCoreScoreType = GetOveralCoreScore(totalSumOfAllCategoryElements);
            return overalCoreScoreType;
        }

        public int GetOveralCoreScore(double totalSumOfAllCategoryElements)
        {
            if (totalSumOfAllCategoryElements < 14.9)
                return Constants.Questioannaire.POOR_GRADE;
            if (totalSumOfAllCategoryElements < 24.9)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (totalSumOfAllCategoryElements < 30)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;
        }

        protected string ConvertToScoreColor(double score)
        {
            if (score == Constants.Questioannaire.POOR_GRADE)
                return Constants.Questioannaire.RED_COLOR;
            if (score == Constants.Questioannaire.AVERAGE_GRADE)
                return Constants.Questioannaire.ORANGE_COLOR;
            if (score == Constants.Questioannaire.GOOD_GRADE)
                return Constants.Questioannaire.GREEN_COLOR;

            return Constants.Questioannaire.LIGHT_GRAY;
        }
         
        #region CoreScore Localization


        public static  CoreScoreAnswer GetLocalizedCoreScore(string locale, double questionnaireAnswer)
        {
            string scoreString = null;
            var coreScore = new CoreScoreAnswer();
            if (Math.Abs(questionnaireAnswer - 1) < 0.01)
                scoreString = "WEAKCORE";
            if (Math.Abs(questionnaireAnswer - 2) < 0.01)
                scoreString = "AVERAGECORE";
            if (Math.Abs(questionnaireAnswer - 3) < 0.01)
                scoreString = "STRONGCORE";
            if (scoreString != null)
            {
                coreScore.SetScoreAnswer(scoreString, locale, GetScoreColor(questionnaireAnswer));
            }
            else
            {
                return GetNullCoreScore();
            }

            return coreScore;
        }
        protected static string GetScoreColor(double score)
        {
            if (Math.Abs(score - 1) < Epsilon)
                return Constants.Questioannaire.RED_COLOR;
            if (Math.Abs(score - 2) < Epsilon)
                return Constants.Questioannaire.ORANGE_COLOR;
            if (Math.Abs(score - 3) < Epsilon)
                return Constants.Questioannaire.GREEN_COLOR;
            return Constants.Questioannaire.LIGHT_GRAY;
        }
        public static CoreScoreAnswer GetNullCoreScore()
        {
            return new CoreScoreAnswer
            {
                CoreScore = "no data",
                Color = Constants.Questioannaire.LIGHT_GRAY
            };
        } 
        #endregion

        #endregion
    }
}
