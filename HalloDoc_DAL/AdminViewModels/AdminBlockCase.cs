using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminBlockCase
    {
        public int RequestId { get; set; }
        public string PatientName { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [MaxLength(500, ErrorMessage = "Could Not Enter More Than 500 Charachters.")]
        public string ReasonForBlocking { get; set; }
    }
}
