using System.Linq;
using AppdevPhong.Data;
using AppdevPhong.Models;
using AppDevPhong.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppdevPhong.Areas.Authenticated.Controllers
{
    [Area(SD.Authenticated_Area)]
    [Authorize(Roles = SD.Role_Staff)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }


        [Authorize(Roles = SD.Role_Staff)]
        [HttpGet]
        public IActionResult Index()
        {
            var categoriesList = _db.Categories.ToList();
            return View(categoriesList);
        }


        [Authorize(Roles = SD.Role_Staff)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var categoryBeDeleted = _db.Categories.Find(id);

            _db.Categories.Remove(categoryBeDeleted);
            _db.SaveChanges();

            //redirect to action Index

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = SD.Role_Staff)]
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            //Create
            if (id == null) return View(new Category());

            //update
            var categories = _db.Categories.Find(id);
            return View(categories);
        }


        [Authorize(Roles = SD.Role_Staff)]
        [HttpPost]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                    // create
                    _db.Categories.Add(category);
                else
                    // Update
                    _db.Categories.Update(category);

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // if these properties not satisfied then return view old object wilt error
            return View(category);
        }
    }
}