using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SP.LoadBalancer.WebAPI.Models
{
    public class UserModel
    {
        public string FirstName { get; set; }
        public string EmailAddress { get; set; }
        public string Title { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }
}