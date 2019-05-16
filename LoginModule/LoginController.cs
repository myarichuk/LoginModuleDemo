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

        public LoginController(IDocumentStore store, string database, bool deployMapReduceStaticIndex = true)
        {
            _store = store;
            _database = database;

            //ensure index is created
            if (deployMapReduceStaticIndex) 
                new CountByRegistrationDate().Execute(store, null, database);
        }

        [HttpPost, Route("users")]
        public IActionResult Login([FromQuery] string email, [FromQuery] string password)
        {
            using (var session = _store.OpenSession(_database))
            {
                var user = session.Query<User>().FirstOrDefault(u => u.Emails.Contains(email));
                if (user == null)
                    return NotFound(email);
                if (Utilities.CheckPasswordMatch(user.HashedPassword, user.Salt, password) == false)
                    return Unauthorized();

                //user is authenticated, do relevant stuff here...
                return Ok();

            }
        }

        [HttpPut, Route("users")]
        public IActionResult RegisterUser(
            [FromQuery] string firstName, [FromQuery] string lastName,
            [FromQuery] string[] emails, [FromQuery] string password)
        {
            using (var session = _store.OpenSession(_database))
            {
                var hashedPassword = Utilities.HashPassword(password, out var salt);
                session.Store(new User
                {
                    Firstname = firstName,
                    Lastname = lastName,
                    Emails = emails,
                    RegisteredAt = DateTime.UtcNow,
                    HashedPassword = hashedPassword,
                    Salt = salt
                });
                session.SaveChanges();
            }

            return Ok();
        }

        [HttpGet, Route("users/registrationdate")]
        public IActionResult CountByRegistrationDateAutoIndex()
        {
            using (var session = _store.OpenSession(_database))
            {
                var queryResults =
                        from user in session.Query<User>()
                        group user by user.RegisteredAt.Date into g
                        select new
                        {
                            Count = g.Count(), 
                            RegistrationDate = g.Key
                        };

                return Ok(queryResults.ToArray());
            }
        }


        [HttpGet, Route("users/registrationdate2")]
        public IActionResult CountByRegistrationDateStaticIndex()
        {
            using (var session = _store.OpenSession(_database))
            {
                var queryResults =
                    session.Query<CountByRegistrationDate.Result, CountByRegistrationDate>()
                        .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(1)))
                        .ToArray();
                return Ok(queryResults);
            }
        }
    }
}
