using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalSchoolGroupsPlatform.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is mandatory!")]
        public string CategoryName { get; set; }

        // Foreign Key Relationship: One (category) - Many (groups)
        public virtual ICollection<Group> Groups { get; set; }
    }
}