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
                .Include(i => i.OfficeAssignment)//Include Offices assigned to instructor
                                                 //================================ Enrollments ===================================
                .Include(i => i.Courses)//within courses property load the enrollments
                  .ThenInclude(i => i.Course)//have to get the course entity out of the Courses join entity
                     .ThenInclude(i => i.Department)
                .OrderBy(i => i.LastName)//Sort by instructor Last name asc
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
                .Include(i=>i.OfficeAssignment) //mwilliams:  include office
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
            //Populate the AssignedCourseData View Model 
            PopulateAssignedCourseData(instructor);
            return View();
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            //get all courses
            var allCourses = _context.Courses;

            //create a hashset of instructor courses (HashSet of integers populated with course id)
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));

            //Create and populate the AssignedCourseData ViewModel
            var viewModel = new List<AssignedCourseData>();//create


            //populate it once for each of the courses within allCourses
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)                    
                });
            }

            //Save the viewModel within the ViewData object for use within View
            ViewData["Courses"] = viewModel;

        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HireDate,LastName,FirstName,Email, OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            //mwilliams:  added string[] selectedCourses method argument for many course assignments

            if (selectedCourses != null)
            {
                //selectedCourses checkboxes have been checked - Create a new list of CourseAssignment
                instructor.Courses = new List<CourseAssignment>();
                //Loop the selectedCourses array
                foreach (var course in selectedCourses)
                {
                    //Populate the CourseAssignment (InstructorID, CourseID)
                    var courseToAdd = new CourseAssignment
                    {
                        InstructorID = instructor.ID,
                        CourseID = int.Parse(course)
                    };
                    instructor.Courses.Add(courseToAdd); //Add the new course to collection
                }
            }
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

            var instructor = await _context.Instructors
                .Include(i=>i.OfficeAssignment)//include office assignment
                .Include(i=>i.Courses)//include courses for AssignedCourseData viewmodel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }
            //Populate the AssignedCourseData View Model 
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
        {
            //mwilliams: Take care of overposting 
            //           Added selectedCourse string arrary argument
            if (id ==null)
            {
                return NotFound();
            }

            //find the instructor to update (because of overposting check)
            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment) //include office assignment 
                .Include(i => i.Courses)//include courses for course assignment
                .ThenInclude(i => i.Course)//for update of course
                .SingleOrDefaultAsync(i => i.ID == id); //only one instructor to update (based on id)

            if(await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,"",i=>i.FirstName, i=>i.LastName, i=>i.HireDate, i=>i.OfficeAssignment))
            {
                //check for emtpy string on office location
                if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;//remove the complete record 
                }


                //Update Courses
                UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                //Save changes (try...catch)
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /*ex*/)
                    {
                        //We could log the error using the ex argument
                        //Let's simply return a model state error back to the view
                        ModelState.AddModelError("", "Unable to save changes.");
                    }
                    return RedirectToAction("Index");
                }
            }
            return View(instructorToUpdate);

        }//end Edit Post

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                //If no checkboxes were selected, initialize the Courses navigation property
                //with an empty collection and return
                instructorToUpdate.Courses = new List<CourseAssignment>();
                return;
            }

            //To facilitate efficient lookups, 2 collections will be stored in HashSet objects
            //: selectedCourseHS ->  selected course (hashset of checkboxe selections)
            //: instructorCourses -> instructor courses (hashset of courses assigned to instructor)
            var selectedCourseHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.Course.CourseID));

            //Loop through all courses in the database and check each course against the ones
            //currently assigned to the instructor versus the ones that were selected in the
            //view
            foreach(var course in _context.Courses)//Loop all courses
            {
                //CONDITION 1:
                //If the checkbox for a course was selected but the course isn't in the 
                //Instructor.Courses navigation property, the course is added to the collection
                //in the navigation property
                if (selectedCourseHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(new CourseAssignment
                        {
                            InstructorID = instructorToUpdate.ID,
                            CourseID = course.CourseID
                        });
                    }
                }
                //CONDITION 2:
                //If the check box for a course wasn't selected, but the course is in the 
                //Instructor.Courses navigation property, the course is removed 
                //from the navigation property.
                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove =
                            instructorToUpdate.Courses
                            .SingleOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }                

            }//end foreach

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
