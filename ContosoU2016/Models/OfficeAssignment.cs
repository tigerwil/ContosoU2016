﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU2016.Models
{
    public class OfficeAssignment
    {
        [Key]
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }
        /*
         * There is a one-to-zero-or-one relationship between the Instructor and 
         * the OfficeAssignment Entities.
         * An OfficeAssignment only exists in relation to the instructor it's assigned to,
         * and therefore it's primary key is also it's foreign key to the Instructor 
         * entity
         * 
         * Problem:  Entity Framework cannot automatically recognize InstructorID as the
         * Primary key of this entity because its name doesn't follow the ID or 
         * ClassnameID naming convention.
         * 
         * Therefore, the key attribute is used to identify it as the key.
         */
         [StringLength(50)]
         [Display(Name ="Office Location")]
        public string Location { get; set; }
        //==================== NAVIGATION PROPERTY ============================== //
        public virtual Instructor Instructor { get; set; }
    }
}
