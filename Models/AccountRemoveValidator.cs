using System.Threading;
using System.ComponentModel.DataAnnotations;

namespace aspnetapp.Models
{
    public class AccountRemoveValidator
    {
        [Required, RegularExpression(@"[0-9A-Za-z]{1,20}", ErrorMessage = "RegexNotMatch")]
        public string? Password { get; set; }
    }
}