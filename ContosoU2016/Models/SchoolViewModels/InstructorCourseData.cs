using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU2016.Models.SchoolViewModels
{
    public class InstructorCourseData
    {
        /*
         *  string query =  "SELECT ID, Email, FirstName + LastName as FullName, Course.CourseID, Title " +
                            "FROM People JOIN CourseAssignment ON ID = InstructorID JOIN " +
                            "Course ON Course.CourseID = CourseAssignment.CourseID " +
                            "WHERE Email = '{0}'"; 
         */

        public int ID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int CourseID { get; set; }
        public string Title { get; set; }

    }
}
