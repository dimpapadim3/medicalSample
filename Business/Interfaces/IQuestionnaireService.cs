using System;
using System.Collections.Generic;
using ErrorClasses;
using Model.Questionnaire;
using ServiceModel.Questionnaire;
using QuestionnaireTemplate = ServiceModel.Questionnaire.QuestionnaireTemplate;

namespace Business.Interfaces
{
    public interface IQuestionnaireService
    {
        List<QuestionnaireTemplate> GetQuestionnairHistoricalViewData(int userId, string locale,
                                                                      DateTime now, int numberToRecords);

        List<QuestionnaireTemplate> GetQuestionnairHistoricalViewData(int userId, string locale,
                                                                      int numberToRecords);

        QuestionnaireTemplate SubmitAnsweredTemplate(int userId, QuestionnaireTemplate questionnaireTemplate, DateTime? getServerDateTime);
        CoreScoreAnswer GetAverageCoreScoreForPeriod(int userId, int i, DateTime currentTime, string defaultQuestionnaireLocale);
        QuestionnaireTemplate GetQuestionnaireTemplate(int i, string defaultQuestionnaireLocale, out GenericError error, DateTime getServerDateTime, int userId);
        List<QuestionnaireAverageSummaryData> GetAverageQuestionnaireViewData(int userId, int period, DateTime currentTime);
        QuestionnaireAnswers ExtractAnwsersFromAnsweredTemplate(QuestionnaireTemplate template);
        QuestionnaireTemplate InstansiateQuestionnaireForSpecificAnswers(QuestionnaireTemplate newtemplate, QuestionnaireAnswers answers);

        CoreScoreAnswer GetLattestCoreScore(long p);
    }
}