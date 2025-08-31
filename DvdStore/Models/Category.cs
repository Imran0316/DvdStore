using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }
    }
}
