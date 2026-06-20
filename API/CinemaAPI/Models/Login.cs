using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models
{
    public class Login
    {
        [Key]
        public int LoginID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
