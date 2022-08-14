using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using aspnetapp.Helpers;

namespace aspnetapp.Entities
{
    public class Token
    {
        [Key, Required]
        public Guid Id { get; set; }
        [Required, MaxLength(64), Column("User")]
        public string UserEmail { get; set; }
        [Required]
        public bool IsBanned { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }

        public Token() {
        }

        public Token(User user, Guid id) {
            this.User = user;
            this.UserEmail = user.Email;
            this.Id = id;
            this.IsBanned = false;
            this.CreatedAt = DateTime.UtcNow;
        }
        public static Token? find(DataContext context, Guid id, bool includeUser = false) {
            if (includeUser) return context.Tokens.Include(t => t.User).FirstOrDefault(t => t.Id == id);
            return context.Tokens.FirstOrDefault(t => t.Id == id);
        }
    }
}