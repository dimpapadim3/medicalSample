using System.Collections.Generic;
using Model.Questionnaire;
using ServiceModel.Questionnaire;
using QuestionnaireTemplate = ServiceModel.Questionnaire.QuestionnaireTemplate;

namespace Business.Questionnaire
{
    public interface IQuestionnaireCoreScoreCalculationAlgorithm
    {
        CoreScoreAnswer AverageCoreScoreForPeriod(string locale, IList<QuestionnaireAnswers> questionnaireAnswerses);
        void CalculateSingleQuestionnaireCoreScoreColor(QuestionnaireTemplate instanciatedTemplate);
        int OveralCoreScoreType(QuestionnaireTemplate instanciatedTemplate);
     }
}