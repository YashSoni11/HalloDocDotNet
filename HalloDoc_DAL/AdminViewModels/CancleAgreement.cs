using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class CancleAgreement
    {

        public string requestId { get; set; }
        public string patientName { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [MaxLength(500, ErrorMessage = "Could Not Enter More Than 500 Charachters.")]
        public string CanclationReason { get; set; }
    }
}
