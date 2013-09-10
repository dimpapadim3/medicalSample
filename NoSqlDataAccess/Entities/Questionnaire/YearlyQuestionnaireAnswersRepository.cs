using Model.Questionnaire;

namespace NoSqlDataAccess.Entities.Questionnaire
{
    public class YearlyQuestionnaireAnswersRepository : NoSqlRepositoryBase<QuestionnaireAnswers>
    {
        public override string CollectionName
        {
            get
            {
                return "YearlyQuestionnaireAnswers";
            }
        }
    }
}