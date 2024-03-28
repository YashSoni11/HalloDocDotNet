using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{

   public class Physicianshifts
    {
    public DateTime Today { get; set; }
       public List<DayWiseShift> dayWiseShifts { get; set; }
    }
    public class DayWiseShift
    {
        public int physicianId { get; set; }

        public int startTime { get; set; }

        public int endTime { get; set; }    

        public int status { get; set; }

    }
}
