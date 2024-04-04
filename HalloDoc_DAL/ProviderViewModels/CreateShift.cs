using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class CreateShift
    {

   
        public List<Region>? regions { get; set; }

        [Required(ErrorMessage ="Region is required.")]
        public int regionId { get; set; }

        public List<Physician>? physicians { get; set; }

        [Required(ErrorMessage = "Physician is required.")]

        public int PhysicianId { get; set; }

        [Required(ErrorMessage ="Shiftdate is required.")]
        
        public DateTime ShiftDate { get; set; }

        [Required(ErrorMessage = "Starttime is required.")]

        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Endtime is required.")]

        public DateTime EndTime { get; set; }  


        public bool IsReapet { get; set; }

        
        public List<SelectedDays>? SelectedDays { get; set; }

        public int RepeatUpto { get; set; }

    }

    public class SelectedDays
    {

        public bool IsSelected { get; set; }

        public int DayId { get; set; }
    }
}
