﻿using FCS_Analysis.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FCS_Analysis.Models.Entities
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        [EmailAddress]
        [Required]
        public string user_email { get; set; }
        [StringLength(100, MinimumLength = 5)]
        [Required]
        public string user_password { get; set; }
        [Required]
        public string user_name { get; set; }
        public string user_avatar { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;
        public string user_role { get; set; } = Constants.ROLE_CUSTOMER;
        [Phone]
        public string user_phone { get; set; }
        public string user_address { get; set; }
        public bool user_activated { get; set; } = true;
    }
}
