using Assignment.Models;
using BAL.Interface;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITask _task;

        public HomeController(ITask task)
        {
            _task = task;
        }

        public IActionResult Index()
        {

           

            return View();
        }


        public IActionResult GetFilterredTask(int currentPage,string Name,int rowsPerPage)
        {
            List<ViewTask> data = _task.GetAllTasks(Name);

            CatagoryViewModel catagoryViewModel = new CatagoryViewModel()
            {
                 totalPages = (int)Math.Ceiling((double)data.Count / rowsPerPage),
                tasks = data.Skip(rowsPerPage * (currentPage - 1)).Take(rowsPerPage).ToList(),
                currentPages = currentPage,
                rowsPerPage = rowsPerPage
               


            };





            return PartialView("_taskTable", catagoryViewModel);
        }


        public  IActionResult GetCatagoriesList(string name)
        {

              List<Category> categories = _task.GetCatagoriesFromName(name);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            string response = JsonConvert.SerializeObject(categories, settings);

            return Json(response);
        }

        public IActionResult DeleteTask(int taskid)
        {

            bool response = _task.DeleteTask(taskid);

            if (response)
            {

                TempData["ShowPositiveNotification"] = "Task Deleted Succefully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthign went wrong!";


            }

            return RedirectToAction("Index");
        }

        public IActionResult AddTaskView()
        {

             AddTask addTask = new AddTask();

            return PartialView("_AddTaskModel", addTask);
        }

        public IActionResult EditTaskView(int id)
        {

            EditTask editTask =  _task.GetEditTaskDetails(id);

            return PartialView("_EditTaskModal", editTask);
        }


        public IActionResult PostAddTask(AddTask addTask)
        {

            bool response = _task.AddTask(addTask);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Task Added Succefully.";

            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthign went wrong!";

            }

            return RedirectToAction("Index");
        }


        public IActionResult PostEditTask(EditTask EditTask)
        {

            bool response = _task.EditTask(EditTask);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Task Edited Succefully.";

            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthign went wrong!";

            }

            return RedirectToAction("Index");
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}