using System;
using System.Collections.Generic;
using System.Text;

namespace SaveTheWorldApp.Models
{
    public class LoginResponse
    {
        public string id { get; set; }
        public DateTime __createdAt { get; set; }
        public DateTime __updatedAt { get; set; }
        public string __version { get; set; }
        public bool __deleted { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool is_admin { get; set; }
        public bool disabled { get; set; }
        public bool deleted { get; set; }
        public object status { get; set; }
        public object notes { get; set; }
        public object created_by { get; set; }
        public object updated_by { get; set; }
    }
}
