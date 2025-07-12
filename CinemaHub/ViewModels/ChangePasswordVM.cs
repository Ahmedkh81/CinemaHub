using System.ComponentModel.DataAnnotations;

namespace CinemaHub.ViewModels
{
    public class ChangePasswordVM
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
