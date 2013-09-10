using Business.Helpers;
using Business.Interfaces;
using Model.Questionnaire;

namespace Business.Questionnaire.Helpers
{
    public class DailyDataHelper : DailyyDataHelper<QuestionnaireSummaryData, QuestionnaireAnswers>
    {
        public override QuestionnaireSummaryData InitializeNewServiceType(QuestionnaireAnswers answers, TimeSpan span)
        {
            return new QuestionnaireSummaryData { Value = answers.QuestionnaireSummaryData.Value };
        }

        public override QuestionnaireSummaryData CreateEmptyServiceType(TimeSpan span)
        {
            return null; // return new QuestionnaireSummaryData() { };
        }
    }
}