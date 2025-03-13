using Microsoft.AspNetCore.Mvc;
using K4M2A.Entities.Model;
using K4M2A.API.Services.Interface;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuestionController
    {
        private readonly ILogger<QuestionController> logger;
        private IQuestion _question;

        public QuestionController(ILogger<QuestionController> logger, IQuestion question)
        {
            this.logger = logger;
            _question = question;
        }

        [HttpGet(Name = "GetOnBoardingQuestion")]
        public async Task<JsonResponse> GetOnBoardingQuestion()
        {
            try
            {
                var response = await _question.GetOnBoardingQuestion();

                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        /*
        [HttpPost(Name = "InsertAnswer")]
        public async Task<JsonResponse> InsertAnswer(AnswerModel req)
        {
            try
            {
                var response = await _question.InsertAnswerAsync(1,req);

                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }*/
    }
}
