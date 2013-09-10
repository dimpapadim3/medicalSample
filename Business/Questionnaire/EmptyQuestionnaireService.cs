using System;
using System.Collections.Generic;
using ServiceModel.Questionnaire;

namespace Business.Questionnaire
{
    /// <summary>
    /// 
    /// </summary>
    internal class EmptyQuestionnaireService : Business.Questionnaire.QuestionnaireService
    {
        public override List<QuestionnaireTemplate> GetQuestionnairHistoricalViewData(int userId, string locale, DateTime now, int numberToRecords)
        {
            return new List<QuestionnaireTemplate>();
        }

        public override List<QuestionnaireTemplate> GetQuestionnairHistoricalViewData(int userId, string locale, int numberToRecords)
        {
            return new List<QuestionnaireTemplate>();
        }
    }
}