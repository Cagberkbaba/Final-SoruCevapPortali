namespace SoruCevapPortali.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; } = null!;


        public int QuestionId { get; set; } 
        public Question Question { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
    }
}
