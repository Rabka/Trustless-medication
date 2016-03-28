using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrustlessClientWeb.Models
{
    public class NewStatement
    {
        public string Key { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        [Display(Name = "The first drug")]
        public string Drug1 { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        [Display(Name = "The other drug")]
        public string Drug2 { get; set; }

        [Display(Name = "Description (a link to a source)")]
        public string Description { get; set; }
    }
}
