using BAL.Interface;
using DAL.Context;
using DAL.Models;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.Reposetary
{
    public class TaskServices:ITask
    {


        private readonly CategoryContext _context;

        public TaskServices(CategoryContext context)
        {
            _context = context;
        }


        public List<ViewTask> GetAllTasks(string name)
        {
            bool IsAllName = false;

            if (string.IsNullOrEmpty(name))
            {
                IsAllName = true;
            }



            List<ViewTask> tasks = _context.Taskmen.Select(q => new ViewTask
            {
                Taskid = q.Taskid,
                Taskname = q.Taskname,
                Assignee = q.Assignee,
                Description = q.Description,
                Duedate = q.Duedate,
                City = q.City,
                Categoryid = _context.Categories.Where(r => r.Id == q.Categoryid).Select(q=>q.Name).FirstOrDefault(),


            }).ToList();


            tasks = tasks.OrderBy(q=>q.Taskid).Where(q => IsAllName || q.Assignee.ToLower().Contains(name.ToLower())).ToList();

            return tasks;

        }


        public List<Category> GetCatagoriesFromName(string name)
        {

            return _context.Categories.Where(q => q.Name.ToLower().Contains(name.ToLower())).ToList();
        }

        public bool DeleteTask(int id)
        {
            try
            {

             Taskman task = _context.Taskmen.FirstOrDefault(q=>q.Taskid == id);

            if (task != null)
            {

                _context.Taskmen.Remove(task);
                _context.SaveChanges();
                return true;
            }


            return false;
            }catch(Exception ex) 
            {
                return false;
            }
        }
        

        public bool AddTask(AddTask addTask)
        {
            try
            {

                Taskman taskman = new Taskman();
                Category category = _context.Categories.FirstOrDefault(q=>q.Name.ToLower() == addTask.catagory.ToLower());  

                if(category == null)
                {
                    Category category1 = new Category();
                    category1.Name = addTask.catagory;

                    _context.Categories.Add(category1);
                    _context.SaveChanges();

                    taskman.Categoryid = (int)_context.Categories.OrderByDescending(q => q.Id).Select(q=>q.Id).FirstOrDefault();
                }
                else
                {
                    taskman.Categoryid =  _context.Categories.Where(q=>q.Name ==  addTask.catagory).Select(q=>q.Id).FirstOrDefault();
                }




                taskman.Taskname = addTask.TaskName;
                taskman.Assignee = addTask.Assignee;
                taskman.Duedate = addTask.DueDate;
                taskman.Description = addTask.Description;
                taskman.City = addTask.city;
               
               
                _context.Taskmen.Add(taskman);  
                _context.SaveChanges();






                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public EditTask GetEditTaskDetails(int id)
        {
            try
            {
                Taskman taskman = _context.Taskmen.FirstOrDefault(q => q.Taskid == id);


                EditTask edittask = new EditTask();


                edittask.TaskId = taskman.Taskid;
                edittask.TaskName = taskman.Taskname;
                edittask.Assignee = taskman.Assignee;
                edittask.catagory = _context.Categories.Where(q=>q.Id ==  taskman.Categoryid).Select(q=>q.Name).FirstOrDefault();
                edittask.DueDate = taskman.Duedate;
                edittask.city = taskman.City;
                edittask.Description = taskman.Description;


                return edittask;
            }catch(Exception ex)
            {
                return new EditTask();
            }
        }


        public bool EditTask(EditTask editTask)
        {
            try
            {

                Taskman taskman = _context.Taskmen.FirstOrDefault(q => q.Taskid == editTask.TaskId);
                Category category = _context.Categories.FirstOrDefault(q => q.Name.ToLower() == editTask.catagory.ToLower());

                if (category == null)
                {
                    Category category1 = new Category();
                    category1.Name = editTask.catagory;

                    _context.Categories.Add(category1);
                    _context.SaveChanges();

                    taskman.Categoryid = (int)_context.Categories.OrderByDescending(q => q.Id).Select(q => q.Id).FirstOrDefault();
                }
                else
                {
                    taskman.Categoryid = _context.Categories.Where(q => q.Name == editTask.catagory).Select(q => q.Id).FirstOrDefault();
                }

                if (taskman != null)
                {

                taskman.Taskname = editTask.TaskName;
                taskman.Assignee = editTask.Assignee;
                taskman.Duedate = editTask.DueDate;
                taskman.Description = editTask.Description;
                taskman.City = editTask.city;
               

                _context.Taskmen.Update(taskman);
                _context.SaveChanges();
                    return true;
                }






                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
