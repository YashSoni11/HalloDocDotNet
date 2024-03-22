using HalloDoc_DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class CLoseCase
    {

        public string? requestId { get; set; }

        public List<ViewDocument>? ViewDocuments { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }   

        public DateTime Birthdate { get; set; }

        public string ConfirmationNumber { get; set; }




    }
}
