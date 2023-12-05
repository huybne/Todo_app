using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Todo_app.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter a description.")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter a description.")]
        public DateTime? DueDate { get; set; }
        [Required(ErrorMessage = "please select a  category.")]

        public string CategoryId { get; set; } = string.Empty;
        [ValidateNever]
        public Category Category { get; set; } = null!;
        [Required(ErrorMessage = " please select a status.")]
        public string StatusId { get; set; } = string.Empty;
        [ValidateNever]
        public Status Status { get; set; } = null!;
        public bool Overdue => StatusId == "open" && DueDate < DateTime.Today;
    }
}
