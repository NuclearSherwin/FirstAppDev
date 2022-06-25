using System;
using System.Linq;
using AppdevPhong.Data;
using AppdevPhong.Models;
using AppDevPhong.Utility;
using AppdevPhong.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppdevPhong.Areas.Authenticated.Controllers
{
    [Area(SD.Authenticated_Area)]
    [Authorize(Roles = SD.Role_Staff)]
    public class AssignmentsController : Controller
    {
        private static int CourseId;
        private readonly ApplicationDbContext _db;

        public AssignmentsController(ApplicationDbContext db)
        {
            _db = db;
        }


        // GET
        [HttpGet]
        public IActionResult Index()
        {
            return View(_db.Courses.Include(c => c.Category).ToList());
        }


        [HttpGet]
        public IActionResult AssignmentTrainer(int id)
        {
            if (id != null) CourseId = Convert.ToInt32(id);

            var assignmentViewModel = new AssignmentViewModel();
            assignmentViewModel.CourseId = CourseId;
            assignmentViewModel.AssignmentList = _db.Assigments.ToList();
            assignmentViewModel.TrainerList = _db.Trainers.ToList();

            return View(assignmentViewModel);
        }

        [HttpGet]
        // input id of trainer
        public IActionResult Assign(string id)
        {
            var assigment = new Assigment
            {
                TrainerId = id,
                CourseId = CourseId
            };
            

            _db.Assigments.Add(assigment);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Delete(string Id)
        {
            var assignment = _db.Assigments.FirstOrDefault(a => a.CourseId == CourseId && a.TrainerId == Id);

            _db.Assigments.Remove(assignment);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}