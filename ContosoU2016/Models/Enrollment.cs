﻿namespace ContosoU2016.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }//PK

        /*
         * The CourseID property is a foreign key, and the corresponding navigation property
         * is Course.  An Enrollment Entity is associated with one Course Entity
         */ 
        public int CourseID { get; set; }//FK
        public int StudentID { get; set; }//FK

        /*
         * The StudentID property is a foreign key, and the corresponding navigation property
         * is Student. An Enrollment entity is associated with one Student entity, 
         * so the property can only hold a single Student entity.
         * 
         * Entity Framework interprets a property as a foreign key property if it's named
         * <navigation property name><primary key property name> for example:
         * StudentID for the Student navigation property, since the Student entity's 
         * primary key is ID (Inherits from Person Entity ID Property in this case)
         * 
         * Foreign key properties can also be named simple <primary key property name> for example,
         * CourseID, since the Course Entity's primary key is CourseID
         * 
         */

        public Grade? Grade { get; set; }//? nullable:  Because we don't get a grade when registering


        // =================  NAVIGATION PROPERTIES ================= //
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }

    //Grade enumeration
    public enum Grade
    {
        A, B, C, D, F
    }
}