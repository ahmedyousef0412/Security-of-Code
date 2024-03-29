﻿using Ganss.Xss;
using Secure_The_Code.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Secure_The_Code.Models
{
    public class CustomerViewModel
    {
     
        public string Key { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

    }
}
