using System.ComponentModel.DataAnnotations;

namespace CinemaHub.ViewModels
{
    public class ForgotPasswordVM
    {
        public int Id { get; set; }
        [Required]
        public string EmailOrUserName { get; set; }
    }
}
