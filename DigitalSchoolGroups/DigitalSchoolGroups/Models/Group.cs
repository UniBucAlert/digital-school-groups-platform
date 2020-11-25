using DigitalSchoolGroups.Models;
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

        [Required(ErrorMessage = "Group title is mandatory!")]
        [StringLength(100, ErrorMessage = 
            "Group title cannot contain more than 100 characters!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Group description is mandatory!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        // Foreign Key "CategoryId", mapped to Category Model 
        // -> no need to explicitly state [Foreign Key]
        [Required(ErrorMessage = "Group category is mandatory!")]
        public int CategoryId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        // Foreign Key Relationship: One (category) - Many (groups)
        public virtual Category Category { get; set; }

        // Foreign Key Relationship: One (group) - Many (messages)
        public virtual ICollection<Message> Messages { get; set; }

        // Declare Categ array for the categories list.
        // Hold as key-value set (SelectListItem).
        public IEnumerable<SelectListItem> Categ { get; set; }
    }
}