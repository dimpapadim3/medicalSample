using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;
using Model.Questionnaire;
using ServiceModel.Questionnaire;
using Question = ServiceModel.Questionnaire.Question;
using QuestionAnwer = ServiceModel.Questionnaire.QuestionAnwer;
using QuestionnaireTemplate = ServiceModel.Questionnaire.QuestionnaireTemplate;

namespace Business.Questionnaire.QuestioannaireTemplates
{
    public class QuestioannaireAnsweredProcessBase
    {
        public int TemplateId { get; set; }

        #region Extract Answers
        public virtual QuestionnaireAnswers ExtractAnwsersFromAnsweredTemplate(ServiceModel.Questionnaire.QuestionnaireTemplate answeredTemplate)
        {
            var givenAnswersToPersist = new QuestionnaireAnswers
            {
                CategoryQuestionsAnswers = new List<Model.Questionnaire.QuestionAnwer>(),
                QuestionsAnswers = new List<Model.Questionnaire.QuestionAnwer>(),
                TemplateId = answeredTemplate.TemplateId
            };


            //Fill category Questions
            answeredTemplate.QuestionCategories.ForEach(qc =>
            {
                if (qc.CategoryQuestion.QuestionAnwer != null)
                {
                    givenAnswersToPersist.CategoryQuestionsAnswers.Add(new Model.Questionnaire.QuestionAnwer
                    {
                        QuestionId = qc.CategoryQuestion.QuestionId,
                        Answer = qc.CategoryQuestion.QuestionAnwer.Answer
                    });
                    qc.CategoryQuestion.QuestionAnwer = GetSpesificQuestionAnswer(qc.CategoryQuestion);

                }
                //Fill Questions Of Each Category 

                if (qc.Questions != null)
                {
                    qc.Questions.ForEach(question =>
                    {
                        if (question.QuestionAnwer != null)
                        {
                            givenAnswersToPersist.QuestionsAnswers.Add(new Model.Questionnaire.QuestionAnwer
                            {
                                QuestionId = question.QuestionId,
                                Answer = question.QuestionAnwer.Answer
                            });
                            question.QuestionAnwer = GetSpesificQuestionAnswer(question);
                        }
                        else
                        {
                            givenAnswersToPersist.QuestionsAnswers.Add(new Model.Questionnaire.QuestionAnwer
                            {
                                QuestionId = question.QuestionId,
                                Answer = -1
                            });
                            question.QuestionAnwer = GetSpesificQuestionAnswer(question);
                        }
                    });
                }
            });

            return givenAnswersToPersist;
        }

        protected virtual QuestionAnwer GetSpesificQuestionAnswer(Question qc)
        {
            PossibleAnswer res = qc.PossivbleAnswers.FirstOrDefault(c => Math.Abs(c.Value - qc.QuestionAnwer.Answer) < 0.01);

            return new QuestionAnwer
            {
                Answer = qc.QuestionAnwer.Answer,
                Description = res == null ? "-" : res.Description,
                //  Color = QuestionnaireCalculationAlgorithm.GetSingleCategoryScoreColor(qc.QuestionAnwer.Answer)
            };
        }
        
        #endregion

        #region Instantiate Answers

        public QuestionnaireTemplate InstansiateQuestionnaireForSpecificAnswers(QuestionnaireTemplate questionnaireTemplate, QuestionnaireAnswers questionnaireAnswers)
        {
            //Fill category Questions
            questionnaireTemplate.QuestionCategories.ForEach(qc =>
            {
                qc.CategoryQuestion.QuestionAnwer =
                    GetInsatnciatedAnswerForQuestion(questionnaireAnswers.CategoryQuestionsAnswers, qc.CategoryQuestion, () => qc.CategoryQuestion.QuestionId);

                //Fill Questions Of Each Category 
                qc.Questions.ForEach(
                    question =>
                    {
                        question.QuestionAnwer = GetInsatnciatedAnswerForQuestion(questionnaireAnswers.QuestionsAnswers, question, () => question.QuestionId);
                    });
            });

         //   questionnaireTemplate.CoreScoreAnswer = new QuestionnaireCalculationAlgorithmWeek3().GetLocalizedCoreScore(Constants.DEFAULT_QUESTIONNAIRE_LOCALE, questionnaireAnswers.CoreScore);
            return questionnaireTemplate;
        }

        private QuestionAnwer GetInsatnciatedAnswerForQuestion(IEnumerable<Model.Questionnaire.QuestionAnwer> answers, Question qc, Func<int> questionIdSelector)
        {
            Model.Questionnaire.QuestionAnwer givenAnswer = answers.FirstOrDefault(a => a.QuestionId == questionIdSelector());
            PossibleAnswer templateAnswer = qc.PossivbleAnswers.FirstOrDefault(pa => givenAnswer != null && Math.Abs((pa.Value + 1) - (givenAnswer.Answer + 1)) < 0.01);

            QuestionAnwer instanciatedAnswer;
            if (templateAnswer != null)
            {
                Debug.Assert(givenAnswer != null, "givenAnswer != null");
                instanciatedAnswer = new QuestionAnwer
                {
                    Description = templateAnswer.Description,
                    Answer = templateAnswer.Value
                    //    Color = _questionnaireCalculationAlgorithm.GetSingleCategoryScoreColor(givenAnswer.Answer)
                };
            }
            else
            {
                instanciatedAnswer = new QuestionAnwer { Description = "-" };
            }
            return instanciatedAnswer;
        }
        
        #endregion
    }
}