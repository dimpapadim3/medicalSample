using System.Collections.Generic;
using System.Linq;
using Model.Questionnaire;
using ServiceModel.Questionnaire;
using QuestionCategory = Model.Questionnaire.QuestionCategory;
using QuestionnaireTemplate = Model.Questionnaire.QuestionnaireTemplate;

namespace Business.Questionnaire.QuestioannaireTemplates
{
    public class QuestioannaireTemplateRegistry
    {
        public QuestioannaireTemplateRegistry()
        {
            AveliableQuestioannaireTemplates = new List<QuestioannaireTemplateBuilderBase>
                {
                    new QuestioannaireTemplateWeek1 { TemplateId = 1 }, 
                    new QuestioannaireTemplateWeek2 { TemplateId = 2 },
                    new QuestioannaireTemplateWeek3 { TemplateId = 3 }
                };
            QuestioannaireAnsweredExtractors = new List<QuestioannaireAnsweredProcessBase>
                {
                    new QuestioannaireAnsweredProcessBase
                    {
                        TemplateId = 1,
                        //QuestionnaireCalculationAlgorithm  = new QuestionnaireCalculationAlgorithm()  
                    }, 
                    new QuestioannaireAnsweredProcessBase
                    { 
                        TemplateId = 2,
                       // QuestionnaireCalculationAlgorithm  = new QuestionnaireCalculationAlgorithm()  
                    },
                    new QuestioannaireAnsweredProcessBase
                    {
                        TemplateId = 3,
                       // QuestionnaireCalculationAlgorithm  = new QuestionnaireCalculationAlgorithm() 
                    }
                };


        }

        IEnumerable<QuestioannaireTemplateBuilderBase> AveliableQuestioannaireTemplates { get; set; }
        IEnumerable<QuestioannaireAnsweredProcessBase> QuestioannaireAnsweredExtractors { get; set; }

        public QuestioannaireTemplateBuilderBase GetAveliableQuestioannaireTemplateBuilder(int templateid)
        {
            return AveliableQuestioannaireTemplates.FirstOrDefault(t => t.TemplateId == templateid);
        }
        public QuestioannaireAnsweredProcessBase GetAveliableQuestioannaireProccesor(int templateid)
        {
            return QuestioannaireAnsweredExtractors.FirstOrDefault(t => t.TemplateId == templateid);
        }
    }

    public abstract class QuestioannaireTemplateBuilderBase
    {
        public int TemplateId { get; set; }

        public string TemplateLocaleName
        {
            get { return "Template" + TemplateId + "." + Common.Constants.DEFAULT_QUESTIONNAIRE_LOCALE; }
        }

        public abstract IQuestionnaireElementsCalculationAlgorithm QuestionnaireCalculationAlgorithm { get; }
        public abstract IQuestionnaireCoreScoreCalculationAlgorithm  QuestionnaireCoreScoreCalculationAlgorithm { get; }
 
        public abstract QuestionnaireTemplate BuildQuestionnaireTemplate();

        //TODO:  push the implementation down 
        public QuestionnaireTemplate BuildSubmitQuestionnaireTemplate()
        {
            return BuildQuestionnaireTemplate();
        }
         
        #region Localization of Template

        public virtual ServiceModel.Questionnaire.QuestionnaireTemplate LocalizeTemplate(QuestionnaireTemplate questionTemplate)
        {
            var serviceTemplate = new ServiceModel.Questionnaire.QuestionnaireTemplate { TemplateId = TemplateId };

            var categoryRes = new QuestionCategories();

            questionTemplate.QuestionCategories.ForEach(c => LocalizeServiceCategory(TemplateLocaleName, categoryRes, c));

            serviceTemplate.QuestionCategories = categoryRes.QuestionCategoriesList;


            return serviceTemplate;
        }

        private void LocalizeServiceCategory(string locale, QuestionCategories categoryRes, QuestionCategory c)
        {
            var questionsRes = new Questions();

            c.Questions.ForEach(q => LocalizeServiceQuestion(locale, questionsRes, q));

            ServiceModel.Questionnaire.QuestionCategory serviceCategory = categoryRes.AddItem(locale,
                                                                                              c.CategoryQuestion
                                                                                               .Description,
                                                                                              c.CategoryQuestion.Help);
            serviceCategory.CategoryQuestion = LocalizeServiceCategoryQuestion(locale, c);
            serviceCategory.Questions = questionsRes.QuestionsList;
        }

        private ServiceModel.Questionnaire.Question LocalizeServiceCategoryQuestion(string locale, QuestionCategory c)
        {
            var questionsRes = new Questions();
            var posibleAnswersRes = new PossibleAnswers();

            ServiceModel.Questionnaire.Question question = questionsRes.GetLocalizedQuestion(locale, c.CategoryQuestion.Description,
                                                                  c.CategoryQuestion.Help, c.CategoryQuestion.QuestionId);
            c.CategoryQuestion.PossivbleAnswers.ForEach(pa => LocalizePosibleAnswer(locale, posibleAnswersRes, pa));

            question.PossivbleAnswers = posibleAnswersRes.PossivbleAnswersList;

            return question;
        }

        private void LocalizeServiceQuestion(string locale, Questions questionsRes, Model.Questionnaire.Question q)
        {
            var posibleAnswersRes = new PossibleAnswers();

            q.PossivbleAnswers.ForEach(pa => LocalizePosibleAnswer(locale, posibleAnswersRes, pa));

            ServiceModel.Questionnaire.Question serviceQuestion = questionsRes.AddItem(locale, q.Description, q.Help, q.QuestionId);

            serviceQuestion.PossivbleAnswers = posibleAnswersRes.PossivbleAnswersList;
        }

        private void LocalizePosibleAnswer(string locale, PossibleAnswers posibleAnswersRes, PossivbleAnswer pa)
        {
            posibleAnswersRes.AddItem(locale, pa.Description, pa.Value, pa.Id);
        }
        
        #endregion
        
    }
}
