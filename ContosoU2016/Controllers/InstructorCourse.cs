using ContosoU2016.Data;
using ContosoU2016.Models;
using ContosoU2016.Models.SchoolViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU2016.Controllers
{
    /*mwilliams:  Part 9 - IDENTITY FRAMEWORK (add role based authorization) */
    [Authorize(Roles = "instructor")]
    public class InstructorCourse : Controller
    {
        private readonly SchoolContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorCourse(SchoolContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        // GET: InstructorCourse
        public async Task<IActionResult> Index(int? courseID)
        {
            //Get currently logged in user
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            //Locate the logged in instructor within the Instructor Entity
            var instructor = await _context.Instructors
                 .Include(s => s.Courses)
                 .AsNoTracking()
                 .SingleOrDefaultAsync(i => i.Email == user.Email);


            ViewData["Instructor"] = instructor.FullName;

            //Populate ViewModel object using ADO.NET code (Using a Raw SQL query that returns other types)
            List<InstructorCourseData> data = new List<InstructorCourseData>();

            //Get database connection
            var conn = _context.Database.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    string query = "SELECT ID, Email, FirstName + LastName as FullName, Course.CourseID, Title " +
                           "FROM Person JOIN CourseAssignment ON ID = InstructorID JOIN " +
                           "Course ON Course.CourseID = CourseAssignment.CourseID " +
                           "WHERE Email = @Email";
                    //   "WHERE Email = 'mwilliams@faculty.contoso.com'";

                    //Create command object
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = query;


                    //Create parameter object
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@Email";
                    parameter.Value = user.Email;
                    command.Parameters.Add(parameter);

                    //Create Reader object
                    DbDataReader reader = await command.ExecuteReaderAsync();

                    //Check if reader has any rows and populate ViewModel
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new InstructorCourseData
                            {
                                ID = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                FullName = reader.GetString(2),
                                CourseID = reader.GetInt32(3),
                                Title = reader.GetString(4)
                            };
                            data.Add(row);
                        }
                    }
                    //Clean up resources
                    reader.Dispose();
                }
            }
            finally
            {
                //Close Database Connection
                conn.Close();
            }


            if (courseID != null)
            {
                var selectedCourse = _context.Courses.Where(x => x.CourseID == courseID).Single();
                var enrollments = _context.Enrollments
                    .Include(e => e.Student)
                    .Where(e => e.CourseID == selectedCourse.CourseID);


                ViewData["enrolled"] = enrollments.ToList();

                ViewData["Course"] = selectedCourse.Title;

            }

            //Return data 
            return View(data.ToList());

        }




        // GET: InstructorCourse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .SingleOrDefaultAsync(e => e.EnrollmentID == id);

            if (enrollment == null)
            {
                return NotFound();
            }
            return View(enrollment);
        }

        //POST: Enrollment/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Find the enrollment record to be updated
            var enrollmentToUpdate = await _context.Enrollments.SingleOrDefaultAsync(s => s.EnrollmentID == id);

            if (await TryUpdateModelAsync<Enrollment>(
                enrollmentToUpdate, "", e => e.EnrollmentID, e => e.CourseID, e => e.Grade, e => e.StudentID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { courseID = enrollmentToUpdate.CourseID });
                    //return RedirectToAction("Index");
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(enrollmentToUpdate);
        }
    }
}