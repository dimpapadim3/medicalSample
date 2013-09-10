using Model.Questionnaire;

namespace Business.Questionnaire
{
    public interface IQuestionnaireElementsCalculationAlgorithm
    {
        string GetSingleCategoryScoreColor(double answer);
        void CalculateSingleQuestionnaireCategoryScores(QuestionnaireAnswers answers);
        QuestionnaireSummaryData CalculateSingleQuestionnaireCategoryScores(ServiceModel.Questionnaire.QuestionnaireTemplate answeredTemplate);
        void CalculateSingleQuestionnaireCategoryColors(ServiceModel.Questionnaire.QuestionnaireTemplate instanciatedTemplate);

    }
}