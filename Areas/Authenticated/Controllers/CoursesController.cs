using System.Collections.Generic;
using System.Linq;
using AppdevPhong.Data;
using AppdevPhong.Models;
using AppDevPhong.Utility;
using AppdevPhong.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppdevPhong.Areas.Authenticated.Controllers
{
    [Area(SD.Authenticated_Area)]
    [Authorize(Roles = SD.Role_Staff)]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CoursesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = SD.Role_Staff)]
        [HttpGet]
        public IActionResult Index()
        {
            var courseList = _db.Courses.Include(x => x.Category).ToList();
            return View(courseList);
        }


        [Authorize(Roles = SD.Role_Staff)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var courseBeDeleted = _db.Courses.Find(id);

            _db.Courses.Remove(courseBeDeleted);
            _db.SaveChanges();

            // redirect to action index of course

            return RedirectToAction(nameof(Index));
        }


        [NonAction]
        private IEnumerable<SelectListItem> CategoriesSelectListItems()
        {
            // get list of categories
            var categories = _db.Categories.ToList();

            //each category it will generate a selectListItem
            var result = categories.Select(x => new SelectListItem
            {
                //display for user
                Text = x.Name,

                // value of select item
                Value = x.Id.ToString()
            });

            return result;
        }


        [Authorize(Roles = SD.Role_Staff)]
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            //create course view model
            var courseVm = new CourseVm();

            // set data for category list
            courseVm.CategoryList = CategoriesSelectListItems();

            // if id == null the create
            if (id == null)
            {
                // set product is new object
                courseVm.Course = new Course();

                return View(courseVm);
            }


            // if there have an id then update
            var courseFromDb = _db.Courses.Find(id);
            courseVm.Course = courseFromDb;

            return View(courseVm);
        }


        [Authorize(Roles = SD.Role_Staff)]
        [HttpPost]
        public IActionResult Upsert(CourseVm courseVm)
        {
            if (ModelState.IsValid)
            {
                if (courseVm.Course.Id == 0)
                    // create
                    _db.Courses.Add(courseVm.Course);

                else
                    _db.Courses.Update(courseVm.Course);

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }


            //provide the data for category list
            courseVm.CategoryList = CategoriesSelectListItems();

            return View(courseVm);
        }
    }
}