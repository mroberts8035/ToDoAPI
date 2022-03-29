using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ToDoController : ApiController
    {

        //Create a connection to the db
        ToDoEntities db = new ToDoEntities();
                
        //GET- /api/todo
        public IHttpActionResult GetToDos()
        {
            //Below we create a list of Entity Framework ToDo objects. In an API, it is best practice to install Entity Framework to the API layer when needing to accomplish this task.
            List<ToDoViewModel> toDos = db.TodoItems.Include("Category").Select(t => new ToDoViewModel()
            {
                //Assign the columns of the ToDo db table to the ToDoViewModel object, so we can use the data (send the data back to requesting app)
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).ToList<ToDoViewModel>();

            //Check the results and handle accordingly below
            if (toDos.Count == 0)
            {
                return NotFound();
            }
            //Everything is good, return the data
            return Ok(toDos); //ToDo are being passed in the response back to the requesting app.

        }//end GetToDo()

        //GET api/todo/id
        public IHttpActionResult GetToDo(int id)
        {
            //Create a new ToDoViewModel object and assign it to the appropriate ToDoItem from the db
            ToDoViewModel resource = db.TodoItems.Include("Category").Where(t => t.TodoId == id).Select(t =>
            new ToDoViewModel()
            {
                //COPY THE ASSIGNMENTS FROM THE GETTODOS() ABOVE
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }            
        }).FirstOrDefault();

            //scopeless if -once the return executes the scopes are closed.
            if (resource == null)
                return NotFound();

            return Ok(resource);

        }//end GetToDo(id)

        //POST - api/todo (HttpPost)
        public IHttpActionResult PostResource(ToDoViewModel toDo)
        {
            //1. Check to validate the object - we need to know that all the data necessary to create a todolistitem is there
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }//end if

            TodoItem newToDo = new TodoItem()
            {
                Action = toDo.Action,
                Done = toDo.Done,
                CategoryId = toDo.CategoryId,                
            };

            //add the record and save changes
            db.TodoItems.Add(newToDo);
            db.SaveChanges();

            return Ok(newToDo);

        }//End Post 

        //PUT - api/Todo (HttpPut)
        public IHttpActionResult PutToDo(ToDoViewModel toDo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //Get the ToDoItem from the db so we can modify it
            TodoItem existingToDo = db.TodoItems.Where(t => t.TodoId == toDo.TodoId).FirstOrDefault();
            //modify the resource
            if (existingToDo != null)
            {
                existingToDo.Action = toDo.Action;
                existingToDo.Done = toDo.Done;
                existingToDo.CategoryId = toDo.CategoryId;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }//end Put

        //DELETE - api/Todo/id (HTTPDelete)
        public IHttpActionResult DeleteResource(int id)
        {
            //Get the todo from the API to make sure there's a resource with this id
            TodoItem toDo = db.TodoItems.Where(t => t.TodoId == id).FirstOrDefault();

            if (toDo != null)
            {
                db.TodoItems.Remove(toDo);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end Delete

        //We use the Dispose() below to dispose of any connections to the database after we are done with them - best practice to handle performance - dispose of the instance of the controller and db connection when we are done with it.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }//end class
}//end namespace
