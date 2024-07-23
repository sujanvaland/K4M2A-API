using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace SpiritualNetwork.API.Services
{

    public class QuestionService : IQuestion
    {
        private readonly IRepository<OnBoardingQuestion> _questionrepository;
        private readonly IRepository<AnswerOption> _optionrepository;
        private readonly IRepository<UserAnswers> _answerrepository;
        public QuestionService(IRepository<OnBoardingQuestion> questionrepository,
            IRepository<AnswerOption> optionrepository,
            IRepository<UserAnswers> answerrepository)
        {
            _questionrepository = questionrepository;
            _answerrepository = answerrepository;
            _optionrepository = optionrepository;
        }

        public async Task<JsonResponse> GetOnBoardingQuestion()
        {
            try
            {
                var randomQuestions =  _questionrepository.Table
                                    .OrderBy(r => Guid.NewGuid()) // Shuffle the records
                                    .Take(10) // Take only one random record
                                    .ToList();

                List<QuestionRes> res = new List<QuestionRes>();
                var options = new List<AnswerOption>();
                foreach (var question in randomQuestions)
                {
                    QuestionRes q = new QuestionRes();
                    q.Question = question;
                    q.Options = _optionrepository.Table.Where(x => x.IsDeleted == false && x.QuestionId == question.Id).ToList();
                    res.Add(q);
                }

                return new JsonResponse(200, true, "Success", res);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> InsertAnswerAsync(int userid, List<AnswerModel> answerModel)
        {
            try
            {
                var data = await _answerrepository.Table.Where(x => x.UserId == userid).ToListAsync();
                if (data != null)
                {
                    await _answerrepository.DeleteRangeAsync(data);
                }
                foreach (var answerModels in answerModel)
                {
                    int qid = answerModels.QuestionId;
                    List<UserAnswers> list = new List<UserAnswers>();

                    foreach (var item in answerModels.AnswersId)
                    {
                        UserAnswers ans = new UserAnswers();
                        ans.QuestionId = qid;
                        ans.UserId = userid;
                        ans.AnswerId = item;
                        ans.DescriptiveAnswer = " ";
                        ans.AttemptNo = 1;
                        list.Add(ans);
                    }
                    await _answerrepository.InsertRangeAsync(list);
                }
                return new JsonResponse(200, true, "Success", null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
