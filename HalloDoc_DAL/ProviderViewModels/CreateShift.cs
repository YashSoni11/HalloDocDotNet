using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class CreateShift
    {


        public List<Region> regions { get; set; }   

        public int regionId { get; set; }
        public List<Physician> physicians { get; set; }    


        public int PhysicianId { get; set; }

        public DateTime ShiftDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }  


        public bool IsReapet { get; set; }

        public List<SelectedDays> SelectedDays { get; set; }

        public int RepeatUpto { get; set; }

    }

    public class SelectedDays
    {
        public bool IsSelected { get; set; }

        public int DayId { get; set; }
    }
}
