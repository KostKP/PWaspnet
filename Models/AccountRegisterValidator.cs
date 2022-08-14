using System.ComponentModel.DataAnnotations;

namespace aspnetapp.Models
{
    public class AccountRegisterValidator
    {
        [Required, RegularExpression(@"([0-9a-z.]{1,32})@([0-9a-z]{1,16})\.([0-9a-z.]{1,8})", ErrorMessage = "RegexNotMatch")]
        public string Email { get; set; }
        [Required, RegularExpression(@"[0-9A-Za-z]{1,20}", ErrorMessage = "RegexNotMatch")]
        public string Password { get; set; }
    }
}