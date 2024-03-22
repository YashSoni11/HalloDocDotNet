using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminAssignCase
    {

      public   List<Region>? Regions { get; set; }

        public List<Physician>?  Physicians { get; set; }

        [Required(ErrorMessage = "Region Is Required")]
        public string SelectedRegionName { get; set; }

        [Required(ErrorMessage ="Physician is Required")]
        public string SelectedPhycisianId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500, ErrorMessage = "Could Not Enter More Than 500 Charachters.")]
        public string Description { get; set; }

        public int? RequestId { get; set; }

         
    }
}
