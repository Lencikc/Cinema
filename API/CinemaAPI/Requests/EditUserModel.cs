namespace CinemaAPI.Requests
{
    public class EditUserModel
    {
        public int UserID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public int RoleID { get; set; }
    }
}
