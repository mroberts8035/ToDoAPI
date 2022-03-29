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
    public class CategoriesController : ApiController
    {

        ToDoEntities db = new ToDoEntities();

        //Get - api/Categories
        public IHttpActionResult GetCategories()
        {
            List<CategoryViewModel> cats = db.Categories.Select(c => new CategoryViewModel()
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).ToList<CategoryViewModel>();

            if (cats.Count == 0)
                return NotFound();

            return Ok(cats);

        }//end GetCategories()

        //GET - api/categories/id
        public IHttpActionResult GetCategory(int id)
        {
            CategoryViewModel cat = db.Categories.Where(c => c.CategoryId == id).Select(c => new CategoryViewModel()
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).FirstOrDefault();

            if (cat == null)
            {
                return NotFound();
            }
            return Ok(cat);
        }//end GetCategory

        //POST - api/categories (HTTPPost)
        public IHttpActionResult PostCategory(CategoryViewModel cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            db.Categories.Add(new Category()
            {
                Name = cat.Name,
                Description = cat.Description
            });

            db.SaveChanges();
            return Ok();
        }//end PostCategory

        //PUT - api/categories (HTTPPut)
        public IHttpActionResult PutCategory(CategoryViewModel cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //Get the resource from the db to be able to modify it
            Category existingCat = db.Categories.Where(c => c.CategoryId == cat.CategoryId).FirstOrDefault();

            //modify the category
            if (existingCat != null)
            {
                existingCat.Name = cat.Name;
                existingCat.Description = cat.Description;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }//end PutCategory

        //api/categories (HTTPDelete)
        public IHttpActionResult DeleteCategory(int id)
        {
            //Get the resource from the API to make sure there's a resource with this id
            Category cat = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();

            if (cat != null)
            {
                db.Categories.Remove(cat);
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
