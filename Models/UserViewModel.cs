namespace PromptEngineering.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public int ReportingId { get; set; }
        public string ReportingTo { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
