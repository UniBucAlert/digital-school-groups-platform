using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalSchoolGroupsPlatform.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public DateTime Date { get; set; }

        // Foreign Key
        // Cuvantul Category din nume face o mapare --> stie ca vine din modelul Category,
        // deci nu mai trebuie sa scriem Foreign Key.
        public int CategoryId { get; set; }

        // relatia de cheie externa
        public virtual Category Category { get; set; }
        //public virtual ICollection<Comment> Comments { get; set; }

        // Denumim Categ (nu Category / Categories), sa nu il confundam!
        // Categ tine o lista de categorii.
        // Tinem cu cheie si valoare (SelectListItem).
        public IEnumerable<SelectListItem> Categ { get; set; }
    }
}