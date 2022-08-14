using System.ComponentModel.DataAnnotations;
using aspnetapp.Helpers;

namespace aspnetapp.Entities
{
    public class User
    {
        [Key, Required, MaxLength(64)]
        public string Email { get; set; }
        [Required, MaxLength(16)]
        public string Role { get; set; }
        public string Password { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<Token> Tokens { get; set; }
        public ICollection<Action> Actions { get; set; }

        public User() {
            Tokens = new List<Token>();
            Actions = new List<Action>();
        }

        public User(string email, string role, string password, DateTime createdAt) : base() {
            this.Email = email;
            this.Role = role;
            this.Password = password;
            this.CreatedAt = createdAt;
        }

        public static User? find(DataContext context, string email, string password) {
            return context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
        public static User? find(DataContext context, string email) {
            return context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}