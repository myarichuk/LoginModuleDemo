using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LoginModule.Model;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Embedded;
using Xunit;

namespace LoginModule.Tests
{
    public class LoginModuleTests : IDisposable
    {
        private readonly IDocumentStore _documentStore;
        private const string DatabaseName = "Users";
        public LoginModuleTests()
        {
            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                AcceptEula = true,
                CommandLineArgs = new List<string>
                {
                    "--RunInMemory=true",
                    "--Setup.Mode=None"
                }
            });
            _documentStore = EmbeddedServer.Instance.GetDocumentStore(DatabaseName);                    
        }

        [Fact]
        public void Can_register_user()
        {
            var loginModule = new LoginController(_documentStore, DatabaseName);
            
            loginModule.RegisterUser("John", "Doe", new[] {"john.doe@foobar.me", "anonymous.mail@foobar.com" }, "foobar password");

            using (var session = _documentStore.OpenSession(DatabaseName))
            {
                var fetchedUser = session.Query<User>()
                    .FirstOrDefault(u => u.Firstname == "John" && u.Lastname == "Doe");

                Assert.NotNull(fetchedUser);
            }
        }

        [Fact]
        public void Can_login_user()
        {
            var loginModule = new LoginController(_documentStore, DatabaseName);            
            loginModule.RegisterUser("John", "Doe", new[] {"john.doe@foobar.me", "anonymous.mail@foobar.com" }, "foobar password");

            var successResult = loginModule.Login("anonymous.mail@foobar.com", "foobar password");
            Assert.IsType<OkResult>(successResult);

            var unauthorizedResult = loginModule.Login("anonymous.mail@foobar.com", "wrong password");
            Assert.IsType<UnauthorizedResult>(unauthorizedResult);
        }

        [Fact]
        public void Can_count_registrations_by_date_static_index()
        {
            var loginModule = new LoginController(_documentStore, DatabaseName);            
            loginModule.RegisterUser("John", "Doe", new[] {"john.doe@foobar.me", "anonymous.mail@foobar.com" }, "foobar password");
            loginModule.RegisterUser("Jack", "Doe", new[] {"jack.doe@foobar.me" }, "foobar password");
            loginModule.RegisterUser("Jane", "Doe", new[] {"jane.doe@foobar.me" }, "foobar password");

            var results = ((OkObjectResult) loginModule.CountByRegistrationDateStaticIndex()).Value as CountByRegistrationDate.Result[];

            Assert.NotNull(results);
            Assert.Single(results);

            Assert.Equal(3, results[0].Count);
            Assert.Equal(DateTime.Now.Date,results[0].RegistrationDate);
        }

        [Fact]
        public void Can_count_registrations_by_date_dynamic_index()
        {
            var loginModule = new LoginController(_documentStore, DatabaseName, false);            
            loginModule.RegisterUser("John", "Doe", new[] {"john.doe@foobar.me", "anonymous.mail@foobar.com" }, "foobar password");
            loginModule.RegisterUser("Jack", "Doe", new[] {"jack.doe@foobar.me" }, "foobar password");
            loginModule.RegisterUser("Jane", "Doe", new[] {"jane.doe@foobar.me" }, "foobar password");

            var results = ((OkObjectResult) loginModule.CountByRegistrationDateAutoIndex()).Value as CountByRegistrationDate.Result[];

            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal(3, results[0].Count);
            Assert.Equal(DateTime.Now.Date,results[0].RegistrationDate);
        }

        public void Dispose()
        {
            EmbeddedServer.Instance.Dispose();
        }
    }
}
