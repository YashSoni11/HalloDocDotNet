using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class ClientRequest
    {
        public string? Firstname { get; set; }

        public string Lastname { get; set; }

        public DateTime Birthdate { get; set; }

        public string Phonenumber { get; set; }

        public string Email { get; set; }

        public string Street { get; set; }

        public string City { get; set; }    

        public string State { get; set; }

        public string Zipcode { get; set; }

        public string Notes { get; set; }

        public int RequestTypeId { get; set; }

        public string ConfirmationNumber { get; set; }

        public int RequestId { get; set; }

    }
}
