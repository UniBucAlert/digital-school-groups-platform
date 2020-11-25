using DigitalSchoolGroups.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalSchoolGroupsPlatform.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required(ErrorMessage = "Message content cannot be left empty!")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int GroupId { get; set; }

        public string UserId { get; set; }
        
        // Foreign Key Relationship: 1 User - M Messages
        public virtual ApplicationUser User { get; set; }

        // Foreign Key Relationship: 1 Group - M Messages
        public virtual Group Group { get; set; }
    }
}