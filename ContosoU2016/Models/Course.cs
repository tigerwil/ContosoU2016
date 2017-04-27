using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoU2016.Models
{
    public class Course
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        //You can turn off IDENTITY Propery by using the DatabaseGeneratedOption.None
        //You have the following 3 options:
        //  Computed: Database generates a value when a row is inserted or updated.
        //  Identity: Database generates a value when a row is inserted
        //  None: Database does not generate a value
        public int CourseID { get; set; }//PK

        public string Title { get; set; }

        public int Credits { get; set; }

        //================= NAVIGATION PROPERTIES =================== //
        //1 course: many enrollments
        public virtual ICollection<Enrollment> Enrollments { get; set; }


    }
}