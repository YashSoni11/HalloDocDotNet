using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminCancleCase
    {

        [Required(ErrorMessage = "Reason is Required.")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "Notes is Required.")]
        [MaxLength(500, ErrorMessage = "Could Not Enter More Than 500 Charachters.")]
        public string AdditionalNotes { get; set; }

        public  int requestId { get; set; }

        public string PatientName { get; set; }

        public List<Casetag> reasons { get; set; }
    }
}
