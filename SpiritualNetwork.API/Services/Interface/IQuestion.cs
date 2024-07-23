using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IQuestion
    {
        public Task<JsonResponse> GetOnBoardingQuestion();
        public Task<JsonResponse> InsertAnswerAsync(int userid, List<AnswerModel> answerModel);
    }
}
