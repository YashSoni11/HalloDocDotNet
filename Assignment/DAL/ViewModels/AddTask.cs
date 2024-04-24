using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AddTask
    {

        [Required(ErrorMessage = "Taskname is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [MinLength(2, ErrorMessage = "Minimum 2 Charachters is Required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string TaskName { get; set; }

        [Required(ErrorMessage = "Assignee is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [MinLength(2, ErrorMessage = "Minimum 2 Charachters is Required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string Assignee { get; set; }

        [Required(ErrorMessage = "Description is Required.")] 
        public  string Description { get; set; }

        [Required(ErrorMessage = "Due date is Required.")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "City is Required.")]
        public string city { get; set; }

        [Required(ErrorMessage = "Catagory is Required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string catagory { get; set; }

         
    }


    public  class EditTask
    {
        [Required(ErrorMessage = "Taskname is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [MinLength(2, ErrorMessage = "Minimum 2 Charachters is Required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string TaskName { get; set; }


        public int TaskId { get; set; }


        [Required(ErrorMessage = "Assignee is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [MinLength(2, ErrorMessage = "Minimum 2 Charachters is Required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string Assignee { get; set; }

        [Required(ErrorMessage = "Description is Required.")]

        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is Required.")]

        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "City is Required.")]

        public string city { get; set; }

        [Required(ErrorMessage = "Catagory is Required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string catagory { get; set; }


    }
}
