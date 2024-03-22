using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class Order
    {

        public List<Healthprofessionaltype>? healthprofessionaltypes;
        public string? RequestId;


        [Required(ErrorMessage = "Profession is Required.")]

        public string Profession { get; set; }

        [Required(ErrorMessage = "Business is Required.")]
        public string Business { get; set; }


        [Required(ErrorMessage = "Businesscontact is required.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Please Enter Valid contact")]
        public string BusinessContact { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Faxnumber is Required.")]
        public string FaxNumber { get; set; }


        [Required(ErrorMessage = "Prescription is Required.")]
        [MaxLength(500, ErrorMessage = "Could Not Enter More Than 500 Charachters.")]
        public string Prescription { get; set; }

        [Required(ErrorMessage = "RefillNumber is Required.")]
        public string RefillNumber { get; set; }
    }
}
