using Microsoft.AspNetCore.Identity;

namespace SoruCevapPortali.Models
{
    public class AppUser :IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = "NoUser.jpg";
        public DateTime CreatedAt { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
