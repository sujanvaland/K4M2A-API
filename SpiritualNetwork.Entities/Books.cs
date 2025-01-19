using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class Books : BaseEntity
    {
        public string BookImg { get; set; }
        public string BookName { get; set; }
        public string Author{ get; set; }
		public string BookId { get; set; }
		public int UserId { get; set; }

    }

    public class Movies : BaseEntity
    {
        public string MovieImg { get; set; }
        public string MovieName { get; set; }
    }

    public class Gurus : BaseEntity
    {
        public string GuruImg { get; set; }
        public string GuruName { get; set; }
    }

    public class Practices : BaseEntity
    {
        public string PracticeImg { get; set; }
        public string PracticeName { get; set; }
    }

    public class Experience : BaseEntity
    {
        public string ExperienceImg { get; set; }
        public string ExperienceName { get; set; }
    }

    public class UserProfileSuggestion : BaseEntity
    {
        public int UserId { get; set; }
        public int SuggestedId { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
    }
}
