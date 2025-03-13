using K4M2A.Entities.Model;
using K4M2A.Entities.CommonModel;

namespace K4M2A.Services.Interface
{
    public interface IQuestion
    {
        public Task<JsonResponse> GetOnBoardingQuestion();
        public Task<JsonResponse> InsertAnswerAsync(int userid, List<AnswerModel> answerModel);
    }
}
