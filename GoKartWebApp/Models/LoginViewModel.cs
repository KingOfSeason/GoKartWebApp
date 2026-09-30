using System.ComponentModel.DataAnnotations;

namespace GoKartWebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter Username or Email")]
        public string Email { get; set; } // Username aur Email dono ke liye kaam karega

        [Required(ErrorMessage = "Please enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}