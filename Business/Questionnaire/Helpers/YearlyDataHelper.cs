using Business.Helpers;
using Business.Interfaces;
using Model.Questionnaire;

namespace Business.Questionnaire.Helpers
{
    public class YearlyDataHelper : YearlyDataHelper<QuestionnaireSummaryData, QuestionnaireAnswers>
    {
        public override QuestionnaireSummaryData InitializeNewServiceType(QuestionnaireAnswers dailyInfoInTimeSpan,
                                                                          TimeSpan span)
        {
            return new QuestionnaireSummaryData { Value = dailyInfoInTimeSpan.QuestionnaireSummaryData.Value };
        }

        public override QuestionnaireSummaryData CreateEmptyServiceType(TimeSpan span)
        {
            return null; //return new QuestionnaireSummaryData() { };
        }
    }
}