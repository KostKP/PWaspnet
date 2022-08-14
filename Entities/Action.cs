using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnetapp.Entities
{
    public class Action
    {
        [Required, MaxLength(64), Column("User")]
        public string UserEmail { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string ActionData { get; set; }

        public User User { get; set; }

        public Action() {
        }

        public Action(User user, DateTime date, string action) {
            this.UserEmail = user.Email;
            this.User = user;
            this.Date = date;
            this.ActionData = action;
        }
    }
}