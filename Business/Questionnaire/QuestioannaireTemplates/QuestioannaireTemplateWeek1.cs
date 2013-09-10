using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common;
using Model.Questionnaire;

namespace Business.Questionnaire.QuestioannaireTemplates
{
    //todo  : this should moved to seperate assemply and used as a plug in that will aslso contain Resource files nad UI Template sepcific controls 
    public class QuestioannaireTemplateWeek1 : QuestioannaireTemplateBuilderBase
    {
        public override IQuestionnaireElementsCalculationAlgorithm QuestionnaireCalculationAlgorithm { get { return new QuestionnaireCalculationAlgorithmWeek1(); } }

        public override IQuestionnaireCoreScoreCalculationAlgorithm QuestionnaireCoreScoreCalculationAlgorithm
        {
            get { return new QuestionnaireCalculationAlgorithmWeek1(); }
        }

        public override QuestionnaireTemplate BuildQuestionnaireTemplate()
        {

            const int nuberOfCategories = 7;
            var numBerOfQuestionsArray = new[] {1, 2, 2, 2, 6, 6, 7 };
            var numBerOfPossibleAnswersArray = new[]
                {    
                    new[] {4},
                    new[] {4,4},
                    new[] {4,4},
                    new[] {4,4},
                    new[] {4,4, 4, 4, 4, 4, 4, 4},
                    new[] {4,4, 4, 4, 4, 4, 4, 4, 4, 4},
                    new[] {4,4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0}
                };


            var template = new QuestionnaireTemplate { TemplateId = TemplateId, };
            var categories = new List<QuestionCategory>();
            int id = 0;

            template.QuestionCategories = categories;


            for (int i = 0; i < 3; i++)
            {
                var bigCategory = new QuestionCategory
                    {
                        CategoryQuestion = new Question
                            {
                                QuestionId = id,
                                Description = ("big" + (i + 1) + "D"),
                                Help = ("big" + (i + 1) + "H"),
                                PossivbleAnswers = new List<PossivbleAnswer>()
                            }
                    };


                for (int bigAnswer = 0; bigAnswer < 3; bigAnswer++)
                {
                    var poassibleAnswer = new PossivbleAnswer
                    {
                        Description = ("big" + (i + 1) + "A" + (bigAnswer + 1).ToString(CultureInfo.InvariantCulture)),
                        Value = bigAnswer,
                    };
                    bigCategory.CategoryQuestion.PossivbleAnswers.Add(poassibleAnswer);
                }


                bigCategory.Questions = new List<Question>();


                categories.Add(bigCategory);


            }

            try
            {



                for (int i = 0; i < nuberOfCategories; i++)
                {
                    id++;

                    var cat = new QuestionCategory
                        {
                            CategoryQuestion = new Question
                                {
                                    QuestionId = id,
                                    Description = ("T1C" + (i + 1) + "D"),
                                    Help = ("T1C" + (i + 1) + "H"),
                                    PossivbleAnswers = new List<PossivbleAnswer>()
                                }
                        };

                    for (int qca = 0; qca < 4; qca++)
                    {
                        var poassibleAnswer = new PossivbleAnswer
                            {
                                Description = ("T1C" + (i + 1) + "A" + (qca + 1).ToString(CultureInfo.InvariantCulture)),
                                Value = qca,
                            };
                        cat.CategoryQuestion.PossivbleAnswers.Add(poassibleAnswer);
                    }

                    int questionNumber = numBerOfQuestionsArray[i];

                    cat.Questions = new List<Question>();

                    for (int j = 0; j < questionNumber; j++)
                    {
                        id++;
                        var question = new Question
                            {
                                QuestionId = id,
                                Description =
                                    ("T1C" + (i + 1) + "Q" + (j + 1).ToString(CultureInfo.InvariantCulture) + "D"),
                                Help = ("T1C" + (i + 1) + "Q" + (j + 1).ToString(CultureInfo.InvariantCulture) + "H"),
                                PossivbleAnswers = new List<PossivbleAnswer>()
                            };
                        for (int k = 0; k < numBerOfPossibleAnswersArray[i][j]; k++)
                        {
                            var answerValue = k;

                            var poassibleAnswer = new PossivbleAnswer
                                {
                                    Description =
                                        ("T1C" + (i + 1) + "Q" + (j + 1).ToString(CultureInfo.InvariantCulture) + "A" +
                                         (k + 1).ToString(CultureInfo.InvariantCulture)),
                                    Value = answerValue,
                                };
                            question.PossivbleAnswers.Add(poassibleAnswer);
                        }

                        cat.Questions.Add(question);
                    }

                    categories.Add(cat);
                }

            }
            catch (Exception)
            {

            }
            template.TemplateId = TemplateId;
            return template;

        }

        private ServiceModel.Questionnaire.QuestionnaireTemplate FillCategoryGeneralDescriptions(ServiceModel.Questionnaire.QuestionnaireTemplate serviceTemplate)
        {
            var generalDescriptions = new[]
                {"Do you know who you are?",
                    "Do you know what you want?",
                    "Are you in control of your life?",
                    "Energy",
                    "General Health",
                    "Biomechanics",
                    "Training",
                    "Nutrition",
                    "Mental Energy",
                    "Recovery"
                };

            for (int i = 0; i < serviceTemplate.QuestionCategories.Count; i++)
            {
                serviceTemplate.QuestionCategories[i].CategoryQuestion.GeneralDescription = generalDescriptions[i];
            }

            return serviceTemplate;
        }

        public override ServiceModel.Questionnaire.QuestionnaireTemplate LocalizeTemplate(QuestionnaireTemplate questionTemplate)
        {
            var loc = base.LocalizeTemplate(questionTemplate);
            loc.QuestionCategories[1].CategoryQuestion.IsHidden = true;
            return FillCategoryGeneralDescriptions(loc);

        }
    }


    public class QuestionnaireCalculationAlgorithmWeek1 : QuestionnaireCalculationAlgorithmBase
    {
        public override void CalculateSingleQuestionnaireCategoryColors(ServiceModel.Questionnaire.QuestionnaireTemplate answeredTemplate)
        {
            for (int p = 0; p < answeredTemplate.QuestionCategories.Count; p++)
            {
                var score = answeredTemplate.QuestionCategories[p].Questions.Sum(q =>
                {
                    if (Math.Abs(q.QuestionAnwer.Answer - 3) < 0.001) return 3;
                    return q.QuestionAnwer.Answer + 1;
                });

                if (p == 0) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(BigQuestion(score));
                if (p == 1) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(BigQuestion(score));
                if (p == 2) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(BigQuestion(score));
                if (p == 3) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(GetEnergyCategoryElementScore(score));
                if (p == 4) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(GetHealthCategoryElementScore(score));
                if (p == 5) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(GetbiomechCategoryElementScore(score));
                if (p == 6) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(GetTrainningCategoryElementScore(score));
                if (p == 7) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(GetRecoveryCategoryElementScore(score));
                if (p == 8) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(GetNutritionCategoryElementScore(score));
                if (p == 9) answeredTemplate.QuestionCategories[p].CategoryQuestion.QuestionAnwer.Color = ConvertToScoreColor(GetMentalCategoryElementScore(score));


            }

        }

        public override QuestionnaireSummaryData CalculateSingleQuestionnaireCategoryScores(ServiceModel.Questionnaire.QuestionnaireTemplate answeredTemplate)
        {
            var data = new QuestionnaireSummaryData() { Value = new List<double>() };


            for (int p = 0; p < answeredTemplate.QuestionCategories.Count; p++)
            {
                var score = answeredTemplate.QuestionCategories[p].Questions.Sum(q =>
                {
                    if (Math.Abs(q.QuestionAnwer.Answer - 3) < 0.001) return 3;
                    return q.QuestionAnwer.Answer + 1;
                });

                if (p == 0) data.Value.Add(BigQuestion(score));
                if (p == 1) data.Value.Add(BigQuestion(score));
                if (p == 2) data.Value.Add(BigQuestion(score));
                if (p == 3) data.Value.Add(GetEnergyCategoryElementScore(score));
                if (p == 4) data.Value.Add(GetHealthCategoryElementScore(score));
                if (p == 5) data.Value.Add(GetbiomechCategoryElementScore(score));
                if (p == 6) data.Value.Add(GetTrainningCategoryElementScore(score));
                if (p == 7) data.Value.Add(GetRecoveryCategoryElementScore(score));
                if (p == 8) data.Value.Add(GetNutritionCategoryElementScore(score));
                if (p == 9) data.Value.Add(GetMentalCategoryElementScore(score));


            }

            return data;


        }

        private int BigQuestion(double score)
        {
            if (score < 1)
                return Constants.Questioannaire.POOR_GRADE;
            if (score < 2)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (score < 3)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;
        }

        private int GetEnergyCategoryElementScore(double score)
        {
            if (score < 1)
                return Constants.Questioannaire.POOR_GRADE;
            if (score < 2)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (score < 3)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;
        }
        private int GetHealthCategoryElementScore(double score)
        {
            if (score <= 2.49)
                return Constants.Questioannaire.POOR_GRADE;
            if (score <= 4.49)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (score <= 6)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;
        }
        private int GetbiomechCategoryElementScore(double score)
        {
            if (score <= 2.49)
                return Constants.Questioannaire.POOR_GRADE;
            if (score <= 4.49)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (score <= 6)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;
        }
        private int GetTrainningCategoryElementScore(double score)
        {
            if (score <= 2.49)
                return Constants.Questioannaire.POOR_GRADE;
            if (score <= 4.49)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (score <= 6)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;
        }
        private int GetRecoveryCategoryElementScore(double score)
        {
            if (score <= 10.49)
                return Constants.Questioannaire.POOR_GRADE;
            if (score <= 17.49)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (score <= 21)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;

        }
        private int GetNutritionCategoryElementScore(double score)
        {
            if (score <= 8.9)
                return Constants.Questioannaire.POOR_GRADE;
            if (score <= 14.9)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (score <= 18)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;

        }
        private int GetMentalCategoryElementScore(double score)
        {
            if (score <= 8.9)
                return Constants.Questioannaire.POOR_GRADE;
            if (score <= 14.9)
                return Constants.Questioannaire.AVERAGE_GRADE;
            if (score <= 18)
                return Constants.Questioannaire.GOOD_GRADE;

            return Constants.Questioannaire.INVALID_GRADE;
        }
    }


}