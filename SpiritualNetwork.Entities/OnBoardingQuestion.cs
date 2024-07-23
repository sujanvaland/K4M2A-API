using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class OnBoardingQuestion : BaseEntity
    {
        [MaxLength(1000)]
        public string Question { get; set; }
        [MaxLength(20)]
        public string Type { get; set; }
        public bool IsDescriptive{ get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [MaxLength(1000)]
        public string ImageUrl { get; set; }
        [MaxLength(100)]
        public string Category { get; set; }
        public decimal Mark { get; set; }
        public int Active { get; set; }
    }

    public class AnswerOption : BaseEntity
    {
        public int QuestionId { get; set; }
        [MaxLength(500)]
        public string Option { get; set; }
        public int IsCorrect { get; set; }
    }

    public class UserAnswers : BaseEntity
    {
        public int UserId { get; set; }
        public int QuestionId { get; set;}
        public int? AnswerId { get; set; }
        [MaxLength(1000)]
        public string? DescriptiveAnswer { get; set; }
        public int AttemptNo { get; set; }
    }
}
