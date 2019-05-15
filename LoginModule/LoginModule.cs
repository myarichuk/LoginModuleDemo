using System;
using LoginModule.Model;
using Raven.Client.Documents;
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Local

namespace LoginModule
{
    public class LoginModule
    {
        private readonly IDocumentStore _store;
        private readonly string _database;

        public LoginModule(IDocumentStore store, string database)
        {
            _store = store;
            _database = database;
        }

        public bool TryLogin(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void RegisterUser(User user)
        {
            throw new NotImplementedException();
        }

        public User GetUserByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public void ValidateUserEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}
