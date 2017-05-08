using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoU2016.Data;
using ContosoU2016.Models;
using ContosoU2016.Models.SchoolViewModels;

namespace ContosoU2016.Controllers
{
    public class InstructorController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorController(SchoolContext context)
        {
            _context = context;    
        }

        // GET: Instructor
        public async Task<IActionResult> Index(int? id, int? courseID)//Add Param for Selected Instructor (id)
                                                                      //Add Param for Selected Course (courseID)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = await _context.Instructors
                .Include(i=>i.OfficeAssignment)//Include Offices assigned to instructor
                //================================ Enrollments ===================================
                .Include(i=>i.Courses)//within courses property load the enrollments
                  .ThenInclude(i=>i.Course)//have to get the course entity out of the Courses join entity
                     .ThenInclude(i=>i.Department)
                .OrderBy(i=>i.LastName)//Sort by instructor Last name asc
                //.AsNoTracking()//Improve performance 
                .ToListAsync();

            //=========================== INSTRUCTOR SELECTED =================================
            if (id != null)//if instructor param (id) is passed in
            {
                //Get the instructor data
                Instructor instructor = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single();//return a Single Instructor Entity 
                //Now get instructor courses
                viewModel.Courses = instructor.Courses.Select(s => s.Course);

                //Get the Instructor name for display in view
                ViewData["InstructorName"] = instructor.FullName;

                //Return the Instructor id (id) back to the view for Highlighting selected row
                ViewData["InstructorID"] = id.Value;//passed in URL parameter
                //OR 
                //ViewData["InstructorID"] = instructor.ID;//current instructor ID property
            }


            //========================= END INSTRUCTOR SELECTED =================================

            //================================ COURSE SELECTED ==================================
            if (courseID != null)
            {
                //Get all enrollments for this course (explicit loading:  loading only if requested)
                _context.Enrollments.Include(i => i.Student)
                    .Where(c => c.CourseID == courseID.Value).Load();
                viewModel.Enrollments = viewModel.Courses
                    .Where(x => x.CourseID == courseID).Single().Enrollments;

                //Only enrollments for a single selected course (courseid = 1045)

                //we do not want all enrollments
                //viewModel.Enrollments = _context.Enrollments;

                ViewData["CourseID"] = courseID.Value; //Pass the CourseID view data back to view

            }

            //================================ END COURSE SELECTED ==============================


            return View(viewModel);
            //Original Scaffolded code
            //return View(await _context.Instructors.ToListAsync());





        }

        // GET: Instructor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .SingleOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructor/Create
        public IActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<CourseAssignment>();
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HireDate,LastName,FirstName,Email, OfficeAssignment")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.SingleOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HireDate,ID,LastName,FirstName,Email")] Instructor instructor)
        {
            if (id != instructor.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(instructor);
        }

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .SingleOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.SingleOrDefaultAsync(m => m.ID == id);
            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
