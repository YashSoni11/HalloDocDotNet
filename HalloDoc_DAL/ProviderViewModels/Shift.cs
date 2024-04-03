using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{


    public class ShiftModel
    {
        public DateTime Today { get; set; }
      
        public  List<Region> regions { get; set; }


    }
    public class Physicianshifts
    {
        public DateTime Today { get; set; }

        public DateTime StartOfWeek { get; set; }   

        public DateTime lastDate { get; set; }

        public List<DayWisePhysicianShifts> dayWiseShifts { get; set; }

        public List<WeekWisePhysicianShifts> weekWiseShifts { get;set; }

        public List<MonthWisePhysicianShifts> monthWiseShifts { get; set; }
    }
    public class ShiftInformation
    {


        public int physicianId { get; set; }

        public string PhysicianName { get; set; }   

        public TimeOnly startTime { get; set; }

        public TimeOnly endTime { get; set; }    

        public int status { get; set; }

        public int dayOfWeek { get; set; }

        public int ShiftId { get; set; }

        public int ShiftDetailId { get; set; }

        public DateTime ShiftDate { get; set; }
    }

    public class WeekWisePhysicianShifts
    {
        public int physicianId { get; set; }

        public string PhysicianName { get; set; }

        public List<ShiftInformation> WeekWiseShiftInformation { get; set; }


    }

    public class DayWisePhysicianShifts
    {
        public int physicianId { get; set; }

        public string PhysicianName { get; set; }

        public List<ShiftInformation> DayWiseShiftInformation { get; set; }
    }

    public class MonthWisePhysicianShifts
    {
      public int DayNumber { get; set; }

        public List<ShiftInformation> MonthWiseShiftInformation { get; set; }


    }


    public class ViewShift
    {

        public int physicianId { get; set; }

        public string Physicianname { get; set; }

        public int regionId { get; set; }   

        public string RegionName { get; set; }

        public TimeOnly startTime { get; set; }

        public TimeOnly endTime { get; set; }

        public int status { get; set; }
        public int ShiftId { get; set; }

        public int ShiftDetailId { get; set; }

        public DateTime ShiftDate { get; set; }

        public List<Region> regions { get; set; }   

        public List<Physician> physicians { get; set; }
    }
}
