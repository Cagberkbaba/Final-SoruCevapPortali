using SoruCevapPortali.Models;
using System.ComponentModel.DataAnnotations;

namespace SoruCevapPortali.ViewModels
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }

        [Required]
        public string? QuestionTitle { get; set; } 

        [Required]
        public string? QuestionText { get; set; } 
        public DateTime CreatedAt { get; set; }
        public ICollection<Answer>? Answers { get; set; }

        [Required]
        public int CategoryId { get; set; } 


        public string? AppUserId { get; set; } 
        public AppUser? AppUser { get; set; } 
    }
}
