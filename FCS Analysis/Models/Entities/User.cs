using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FCS_Analysis.Models.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        [EmailAddress]
        public string user_email { get; set; }
        [StringLength(5)]
        public string user_password { get; set; }
        public string user_name { get; set; }
        public string user_avatar { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string user_role { get; set; }
        [Phone]
        public string user_phone { get; set; }
        public string user_address { get; set; }
        public bool user_activated { get; set; }
    }
}
