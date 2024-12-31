namespace PromptEngineering.Models
{
    public class RoleModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsManager { get; set; }
        public bool IsAdmin { get; set;}
    }
}
