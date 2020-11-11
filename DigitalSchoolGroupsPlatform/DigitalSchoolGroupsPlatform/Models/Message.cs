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
        [Required]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int GroupId { get; set; }

        // 1 Group - M Messages
        public virtual Group Group { get; set; }
    }
}