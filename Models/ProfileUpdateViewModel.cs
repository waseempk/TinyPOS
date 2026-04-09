using System.ComponentModel.DataAnnotations;

namespace TinyPOSApp.Models
{
    public class ProfileUpdateViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? CurrentPin { get; set; }

        [MinLength(4, ErrorMessage = "PIN must be at least 4 digits.")]
        public string? NewPin { get; set; }

        [Compare("NewPin", ErrorMessage = "The new PIN and confirmation PIN do not match.")]
        public string? ConfirmNewPin { get; set; }

        public string? ImageUrl { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? ProfileImage { get; set; }

        public bool RemoveImage { get; set; }
    }
}
