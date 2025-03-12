using SpiritualNetwork.Entities;

namespace SpiritualNetwork.Entities.Model
{
    public class QuestionRes
    {
        public OnBoardingQuestion Question { get; set; }
        public List<AnswerOption> Options { get; set; }
    }
}
