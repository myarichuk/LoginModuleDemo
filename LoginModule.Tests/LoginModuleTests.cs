using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LoginModule.Model;
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
            
            loginModule.RegisterUser("John", "Doe", "john.doe@foobar.me","foobar password");

            using (var session = _documentStore.OpenSession(DatabaseName))
            {
                var fetchedUser = session.Query<User>()
                    .FirstOrDefault(u => u.Firstname == "John" && u.Lastname == "Doe");

                Assert.NotNull(fetchedUser);
            }
        }

        public void Dispose()
        {
            EmbeddedServer.Instance.Dispose();
        }
    }
}
