using K4M2A.Entities;

namespace K4M2A.Entities.Model
{
    public class QuestionRes
    {
        public OnBoardingQuestion Question { get; set; }
        public List<AnswerOption> Options { get; set; }
    }
}
