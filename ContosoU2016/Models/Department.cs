using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU2016.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }//PK

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]//client only
        [Column(TypeName ="money")] //SQL Server money data type
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }

        //Relationship to Instructor
        //A department MAY have an Administrator (Instructor), and an 
        //Administrator is always an Instructor
        public int? InstructorID { get; set; }//? denotes nullable

        // ============= NAVIGATION PROPERTIES ==================//
        //Administrator is always an Instructor
        public virtual Instructor Administrator { get; set; }

        //One department with many courses (1:many)
        public virtual ICollection<Course> Courses { get; set; }

        //TO DO :  Handle Concurrency conflicts (Add optimistic Concurrency)

    }
}
