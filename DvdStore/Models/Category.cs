using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain alphabets and spaces")]
        public string CategoryName { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }
    }
}
