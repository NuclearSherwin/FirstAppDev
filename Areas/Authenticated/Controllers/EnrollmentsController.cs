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
    public class EnrollmentsController : Controller
    {
        private static int CourseId;
        private readonly ApplicationDbContext _db;

        public EnrollmentsController(ApplicationDbContext db)
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
        public IActionResult EnrollTrainee(int id)
        {
            if (id != null) CourseId = Convert.ToInt32(id);

            var enrollmentViewModel = new EnrollmentViewModel();
            enrollmentViewModel.CourseId = CourseId;
            enrollmentViewModel.EnrollmentsList = _db.Enrollments.ToList();
            enrollmentViewModel.Trainees = _db.Trainees.ToList();

            return View(enrollmentViewModel);
        }

        [HttpGet]
        public IActionResult Enroll(string id)
        {
            var enrollment = new Enrollment
            {
                TraineeId = id,
                CourseId = CourseId
            };

            _db.Enrollments.Add(enrollment);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Delete(string Id)
        {
            var enrollment = _db.Enrollments.FirstOrDefault(e => e.CourseId == CourseId && e.TraineeId == Id);

            _db.Enrollments.Remove(enrollment);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}