using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bogus;
using SP.LoadBalancer.WebAPI.Models;

namespace SP.LoadBalancer.WebAPI.Services
{
    public class UserService
    {
        public List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();

            for (int i = 0; i < 10; i++)
            {
                var faker = new Faker<UserModel>()
                .RuleFor(u => u.EmailAddress, f => f.Person.Email)
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.FullName, f => f.Person.FullName);
                users.Add(faker.Generate());
            }

            return users;
        }
    }
}