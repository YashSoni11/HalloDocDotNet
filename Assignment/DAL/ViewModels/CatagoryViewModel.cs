using DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class CatagoryViewModel
    { 

         public List<ViewTask> tasks { get; set; }

       public int currentPages { get; set; }

      public  int totalPages { get; set; }

        public int rowsPerPage { get; set; }


    }
    public class ViewTask 
    {


     
        public int Taskid { get; set; }

        public string? Taskname { get; set; }

      
        public string? Assignee { get; set; }

       
        public string? Categoryid { get; set; }

        
        public string? Description { get; set; }

       
        public DateTime? Duedate { get; set; }

       
        public string? City { get; set; }
    }

}
