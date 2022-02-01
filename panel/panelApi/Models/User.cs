using System.ComponentModel.DataAnnotations.Schema;

namespace panelApi.Models
{
    public class User:IEntity
    {
        public int Id { get; set; }
        public string UserName{ get; set; }
        public string Password { get; set; }
        public string ResetPassword { get; set; }
        public int RoleId { get; set; }
        [NotMapped]
        public string Token { get; set; }
    }
}
