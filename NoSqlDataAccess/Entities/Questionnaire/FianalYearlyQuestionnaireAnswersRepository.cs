 using DataAccess.Interfaces;
using Model.Questionnaire;

namespace NoSqlDataAccess.Entities.Questionnaire
{
    public class FianalYearlyQuestionnaireAnswersRepository : NoSqlRepositoryBase<QuestionnaireAnswers>, INoSqlFinalValuesRepositoryBase<QuestionnaireAnswers>
    {
        public override string CollectionName
        {
            get
            {
                return "FianalYearlyQuestionnaireAnswers";
            }
        }
    }
}