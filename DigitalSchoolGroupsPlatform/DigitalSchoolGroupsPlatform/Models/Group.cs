using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalSchoolGroupsPlatform.Models
{
    // Class defining a school group.
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }

        // Foreign Key "CategoryId", mapped to Category Model 
        // -> no need to explicitly state [Foreign Key]
        public int CategoryId { get; set; }

        // Foreign Key Relationship: One (category) - Many (groups)
        public virtual Category Category { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        // Declare Categ array for the categories list.
        // Hold as key-value set (SelectListItem).
        public IEnumerable<SelectListItem> Categ { get; set; }
    }
}