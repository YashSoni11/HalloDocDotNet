using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class NotesView
    {
         public RequestNotes requestnotes { get; set; }

        [Required(ErrorMessage = "Notes is Required!")]
        public string AdditionalNotes { get; set; }

        public int? requestId { get; set; }
    }
}
