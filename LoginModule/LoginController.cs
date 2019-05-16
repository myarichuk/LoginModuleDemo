using LoginModule.Model;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using System;
using System.Linq;
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Local

namespace LoginModule
{
    public class LoginController : ControllerBase
    {
        private readonly IDocumentStore _store;
        private readonly string _database;

        public LoginController(IDocumentStore store, string database)
        {
            _store = store;
            _database = database;
        }

        [HttpPost, Route("users")]
        public IActionResult Login([FromQuery]string email, [FromQuery]string password)
        {
            using (var session = _store.OpenSession(_database))
            {
                var user = session.Query<User>().FirstOrDefault(u => u.Email == email);
                if (user == null)
                    return NotFound(email);
                if (Utilities.CheckPasswordMatch(user.HashedPassword, user.Salt, password))
                {
                    //user is authenticated, do relevant stuff here...
                    return Ok();
                }

                return Unauthorized();
            }
        }

        [HttpPut, Route("users")]
        public IActionResult RegisterUser(
            [FromQuery]string firstName, [FromQuery]string lastName, 
            [FromQuery]string email, [FromQuery]string password)
        {
            using (var session = _store.OpenSession(_database))
            {
                var hashedPassword = Utilities.HashPassword(password, out var salt);
                session.Store(new User
                {
                    Firstname = firstName,
                    Lastname = lastName,
                    Email = email,
                    RegisteredAt = DateTime.UtcNow,
                    HashedPassword = hashedPassword,
                    Salt = salt
                });
                session.SaveChanges();
            }
            return Ok();
        }

        [HttpGet, Route("users")]
        public IActionResult GetUserByUsername([FromQuery]string email)
        {
            using (var session = _store.OpenSession(_database))
            {  
                return Ok(session.Query<User>().FirstOrDefault(u => u.Email == email));
            }
        }
    }
}
