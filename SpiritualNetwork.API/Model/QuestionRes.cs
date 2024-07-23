using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class QuestionRes
    {
        public OnBoardingQuestion Question { get; set; }
        public List<AnswerOption> Options { get; set; }
    }
}
