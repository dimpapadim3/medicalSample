using Model.Questionnaire;

namespace NoSqlDataAccess.Entities.Questionnaire
{
    public class MonthlyQuestionnaireAnswersRepository : NoSqlRepositoryBase<QuestionnaireAnswers>
    {
        public override string CollectionName
        {
            get
            {
                return "MonthlyQuestionnaireAnswers";
            }
        }
    }
}