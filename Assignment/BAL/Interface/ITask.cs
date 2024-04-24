using DAL.Models;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface ITask
    {

        public List<ViewTask> GetAllTasks(string name);

        public bool DeleteTask(int id);

        public List<Category> GetCatagoriesFromName(string name);

        public bool AddTask(AddTask addTask);

        public bool EditTask(EditTask EditTask);

        public EditTask GetEditTaskDetails(int id);

    }
}
