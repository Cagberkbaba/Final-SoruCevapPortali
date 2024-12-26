using SoruCevapPortali.Models;

namespace SoruCevapPortali.ViewModels
{
    public class UserModel
    {
        public string? Id { get; set; }
      
        public string? FullName { get; set; }
        public string? UserName { get; set; }
      
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Role { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public DateTime CreatedAt { get; set; }
        public int QuestionCount { get; set; }
    }
}
