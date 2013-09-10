using Model.Questionnaire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlDataAccess.Entities.Questionnaire
{
    public class QuestionnaireAnswersRepository:NoSqlRepositoryBase<QuestionnaireAnswers>
    {
        public override string CollectionName
        {
            get
            {
                return "QuestionnaireAnswers";
            }
        }
    }

}
