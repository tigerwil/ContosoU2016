﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU2016.Models
{
    public abstract class Person
    {
        //mwilliams:  Create the data models 
        public int ID { get; set; }

        [Required]
        [StringLength(65,ErrorMessage ="Last name cannot be longer than 65 characters.")]
        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50,ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //FullName:  Calculated property with a get accessor only  
        //          - will not get generated in database
        [Display(Name ="Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }


    }
}
