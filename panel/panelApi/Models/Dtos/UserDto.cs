namespace panelApi.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ResetPassword { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string Salt { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }
    }
}
