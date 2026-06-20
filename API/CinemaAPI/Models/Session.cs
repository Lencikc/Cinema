using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaAPI.Models
{
    public class Session
    {
        [Key]
        public int SessionID { get; set; }
        public string Token { get; set; }
        public DateTime LogedAt { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
