using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContosoU2016.Models;
using ContosoU2016.Data;

namespace ContosoU2016.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;

        public HomeController(SchoolContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        private List<Instructor> GetFaculty(string searchString)
        {
            return _context.Instructors
                .Where(i => i.LastName.Contains(searchString) ||
                  i.FirstName.Contains(searchString)).ToList();
        }

        public ActionResult FacultySearch(string q)
        {
            //call getfaculty
            var faculty = GetFaculty(q);
            return PartialView("_FacultySearch", faculty);
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
