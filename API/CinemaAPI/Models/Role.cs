using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
