﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class Documents
    {

       public int requestId { get; set; }
       public  List<ViewDocument> ViewDocuments {  get; set; }

     public   List<IFormFile> FormFile { get; set; }

        public string PatientName { get; set; }

        public string ConfirmationNumber { get; set; }




    }
}
