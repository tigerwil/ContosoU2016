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
          public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }

        //FullName:  Calculated property with a get accessor only  
        //          - will not get generated in database
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }


    }
}
