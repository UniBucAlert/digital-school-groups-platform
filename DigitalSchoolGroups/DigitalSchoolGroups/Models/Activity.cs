using DigitalSchoolGroups.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalSchoolGroupsPlatform.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Title cannot be left empty!")]
        [StringLength(100, ErrorMessage =
            "Activity title cannot contain more than 100 characters!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description cannot be left empty!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        // Pentru a afisa doar data se poate adauga o astfel de validare,
        // chiar daca atributul este de tip DateTime.
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int GroupId { get; set; }

        public string UserId { get; set; }

        // Foreign Key Relationship: 1 User - M Messages
        public virtual ApplicationUser User { get; set; }

        // Foreign Key Relationship: 1 Group - M Messages
        public virtual Group Group { get; set; }
    }
}