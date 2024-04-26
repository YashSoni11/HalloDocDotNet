using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public  class AddressModel
    {


        public string? RoomNo { get; set; }

        [Required(ErrorMessage ="Street is Required.")]
        public string Street { get; set; }

        [Required(ErrorMessage ="City is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is Required.")]
        public int State { get; set; }


        [Required(ErrorMessage = "Zipcode is Required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "ZIP code must be exactly 6 digits.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "ZIP code must be exactly 6 digits.")]
        public string ZipCode { get; set; }
    }
}
