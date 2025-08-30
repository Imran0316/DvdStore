using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime Created_At { get; set; }
        public bool Status { get; set; }


    }
}
