using System.Collections.Generic;
using System.Globalization;
using Model.Questionnaire;

namespace Business.Questionnaire
{
    internal class QuestionnaireBuilder
    {
        public int TemplateId { get { return 1; } }

        public QuestionnaireTemplate BuildQuestionnaireTemplate()
        {

            const int nuberOfCategories = 7;
            var numBerOfQuestionsArray = new[] { 0, 0, 1, 1, 7, 9, 11 };
            var numBerOfPossibleAnswersArray = new[]
                {
                    new[] {0},
                    new[] {0},
                    new[] {4},
                    new[] {4},
                    new[] {4, 4, 4, 4, 4, 4, 4},
                    new[] {4, 4, 4, 4, 4, 4, 4, 4, 4},
                    new[] {4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0}
                };


            var template = new QuestionnaireTemplate { TemplateId = 1, };
            var categories = new List<QuestionCategory>();
            int id = 0;

            template.QuestionCategories = categories;
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
                            Value = qca == 3 ? 2.5 : qca,
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
                            Description = ("T1C" + (i + 1) + "Q" + (j + 1).ToString(CultureInfo.InvariantCulture) + "D"),
                            Help = ("T1C" + (i + 1) + "Q" + (j + 1).ToString(CultureInfo.InvariantCulture) + "H"),
                            PossivbleAnswers = new List<PossivbleAnswer>()
                        };
                    for (int k = 0; k < numBerOfPossibleAnswersArray[i][j]; k++)
                    {
                        var answerValue = k;
 
                        var poassibleAnswer = new PossivbleAnswer
                            {
                                Description = ("T1C" + (i + 1) + "Q" + (j + 1).ToString(CultureInfo.InvariantCulture) + "A" + (k + 1).ToString(CultureInfo.InvariantCulture)),
                                Value = answerValue,
                            };
                        question.PossivbleAnswers.Add(poassibleAnswer);
                    }

                    cat.Questions.Add(question);
                }

                categories.Add(cat);
            }


            return template;
        }
    }
}