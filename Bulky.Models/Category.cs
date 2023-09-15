using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models
{
    public class Category
    {
        [Key] // defining the next field as primary key
        public int Id { get; set; }

        [Required] // defining the following field as Required (This will have a not null setting because it can't be null)
        [DisplayName("Category Name")] // Make the name that will display "Category Name" and not "Name"
        [MaxLength(30)] // added for validation
        public string Name { get; set; }

        [DisplayName("Display Order")] // Make the name that will display "Display Order" and not "Display"
        [Range(1, 100, ErrorMessage = "Display Order must be between 1 - 100.")] // Added for validation
        public int DisplayOrder { get; set; }
    }
}
