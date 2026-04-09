using System.ComponentModel.DataAnnotations;

namespace TinyPOSApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Security PIN is required.")]
        [DataType(DataType.Password)]
        public string Pin { get; set; } = string.Empty;
    }
}
